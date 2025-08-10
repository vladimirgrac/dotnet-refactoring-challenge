using FluentAssertions;
using FluentResults;
using Moq;
using RefactoringChallenge.Bussiness.Services;
using RefactoringChallenge.Infrastructure.Abstractions;
using RefactoringChallenge.Model.Domain;

namespace RefactoringChallenge.BussinessTests.Services;

public class OrderServiceTests
{
    private readonly Mock<ICustomerRepository> _customerRepo;
    private readonly Mock<IOrderRepository> _orderRepo;
    private readonly Mock<IOrderLogsRepository> _orderLogsRepo;
    private readonly Mock<IProductRepository> _productRepo;

    private readonly OrderService _sut;

    public OrderServiceTests()
    {
        _customerRepo = new Mock<ICustomerRepository>();
        _orderRepo = new Mock<IOrderRepository>();
        _orderLogsRepo = new Mock<IOrderLogsRepository>();
        _productRepo = new Mock<IProductRepository>();

        _sut = new OrderService(
            _customerRepo.Object,
            _orderRepo.Object,
            _orderLogsRepo.Object,
            _productRepo.Object
        );
    }

    [Fact]
    public async Task ProcessCustomerOrders_InvalidCustomerId_ReturnsFailure()
    {
        // Act
        var result = await _sut.ProcessCustomerOrders(0);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task ProcessCustomerOrders_CustomerNotFound_ReturnsFailure()
    {
        // Arrange
        _customerRepo
            .Setup(x => x.GetCustomerAsync(It.IsAny<int>()))
            .ReturnsAsync(Result.Fail<Customer>("Not found"));

        // Act
        var result = await _sut.ProcessCustomerOrders(1);

        // Assert
        result.IsFailed.Should().BeTrue();
        _orderRepo.Verify(x => x.GetPendingOrdersAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task ProcessCustomerOrders_PendingOrdersFail_ReturnsFailure()
    {
        // Arrange
        _customerRepo
            .Setup(x => x.GetCustomerAsync(It.IsAny<int>()))
            .ReturnsAsync(Result.Ok(new Customer { IsVip = false, RegistrationDate = DateTime.UtcNow }));

        _orderRepo
            .Setup(x => x.GetPendingOrdersAsync(It.IsAny<int>()))
            .ReturnsAsync(Result.Fail<IEnumerable<Order>>("DB error"));

        // Act
        var result = await _sut.ProcessCustomerOrders(1);

        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task ProcessCustomerOrders_AllProductsAvailable_OrderReady()
    {
        // Arrange
        var order = new Order { Id = 123, Items = new List<OrderItem>() };
        var item = new OrderItem { ProductId = 10, Quantity = 1 };
        order.Items.Add(item);

        _customerRepo
            .Setup(x => x.GetCustomerAsync(It.IsAny<int>()))
            .ReturnsAsync(Result.Ok(new Customer { IsVip = true, RegistrationDate = new DateTime(2020, 1, 1) }));

        _orderRepo
            .Setup(x => x.GetPendingOrdersAsync(It.IsAny<int>()))
            .ReturnsAsync(Result.Ok<IEnumerable<Order>>(new List<Order> { order }));

        _orderRepo
            .Setup(x => x.GetOrderItemsAsync(order.Id))
            .ReturnsAsync(Result.Ok<IEnumerable<OrderItem>>(new List<OrderItem> { item }));

        _productRepo
            .Setup(x => x.GetProductAsync(item.ProductId))
            .ReturnsAsync(Result.Ok(new Product { Id = 10 }));

        _productRepo
            .Setup(x => x.GetStockQuantityAsync(item.ProductId))
            .ReturnsAsync(Result.Ok<int?>(5));

        // Act
        var result = await _sut.ProcessCustomerOrders(1);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainSingle(o => o.Status == "Ready");

        _orderRepo.Verify(x => x.UpdateOrderStatusAsync(order.Id, "Ready"), Times.Once);
        _orderLogsRepo.Verify(x => x.SaveLogAsync(order.Id, It.Is<string>(msg => msg.Contains("Order completed"))), Times.Once);
    }

    [Fact]
    public async Task ProcessCustomerOrders_ProductUnavailable_OrderOnHold()
    {
        // Arrange
        var order = new Order { Id = 123, Items = new List<OrderItem>() };
        var item = new OrderItem { ProductId = 10, Quantity = 10 };
        order.Items.Add(item);

        _customerRepo
            .Setup(x => x.GetCustomerAsync(It.IsAny<int>()))
            .ReturnsAsync(Result.Ok(new Customer { IsVip = false, RegistrationDate = DateTime.UtcNow }));

        _orderRepo
            .Setup(x => x.GetPendingOrdersAsync(It.IsAny<int>()))
            .ReturnsAsync(Result.Ok<IEnumerable<Order>>(new List<Order> { order }));

        _orderRepo
            .Setup(x => x.GetOrderItemsAsync(order.Id))
            .ReturnsAsync(Result.Ok<IEnumerable<OrderItem>>(new List<OrderItem> { item }));

        _productRepo
            .Setup(x => x.GetProductAsync(item.ProductId))
            .ReturnsAsync(Result.Ok(new Product { Id = 10 }));

        _productRepo
            .Setup(x => x.GetStockQuantityAsync(item.ProductId))
            .ReturnsAsync(Result.Ok<int?>(0)); // Nedostupné

        // Act
        var result = await _sut.ProcessCustomerOrders(1);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainSingle(o => o.Status == "OnHold");

        _orderRepo.Verify(x => x.UpdateOrderStatusAsync(order.Id, "OnHold"), Times.Once);
        _orderLogsRepo.Verify(x => x.SaveLogAsync(order.Id, It.Is<string>(msg => msg.Contains("Order on hold"))), Times.Once);
    }

    [Fact]
    public async Task ProcessCustomerOrders_VipCustomerLargeOrder_AppliesMax25PercentDiscount()
    {
        // Arrange
        var order = new Order { Id = 1, Items = new List<OrderItem>() };
        var expensiveItem = new OrderItem { ProductId = 1, Quantity = 10, Product = new Product { Price = 25000 } };
        order.Items.Add(expensiveItem);

        _customerRepo
            .Setup(x => x.GetCustomerAsync(It.IsAny<int>()))
            .ReturnsAsync(Result.Ok(new Customer { IsVip = true, RegistrationDate = new DateTime(2015, 1, 1) }));

        _orderRepo
            .Setup(x => x.GetPendingOrdersAsync(It.IsAny<int>()))
            .ReturnsAsync(Result.Ok<IEnumerable<Order>>(new List<Order> { order }));

        _orderRepo
            .Setup(x => x.GetOrderItemsAsync(order.Id))
            .ReturnsAsync(Result.Ok<IEnumerable<OrderItem>>(order.Items));

        _productRepo
            .Setup(x => x.GetProductAsync(It.IsAny<int>()))
            .ReturnsAsync(Result.Ok(expensiveItem.Product));

        _productRepo
            .Setup(x => x.GetStockQuantityAsync(It.IsAny<int>()))
            .ReturnsAsync(Result.Ok<int?>(100));

        // Act
        var result = await _sut.ProcessCustomerOrders(1);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Single().DiscountPercent.Should().Be(15);
        result.Value.Single().Status.Should().Be("Ready");
    }

    [Fact]
    public async Task ProcessCustomerOrders_RegularCustomerSmallOrder_AppliesLoyaltyDiscount()
    {
        // Arrange
        var order = new Order { Id = 3, Items = new List<OrderItem>() };
        var smallItem = new OrderItem { ProductId = 2, Quantity = 1, Product = new Product { Price = 800 } };
        order.Items.Add(smallItem);

        _customerRepo
            .Setup(x => x.GetCustomerAsync(It.IsAny<int>()))
            .ReturnsAsync(Result.Ok(new Customer { IsVip = false, RegistrationDate = new DateTime(2023, 3, 15) }));

        _orderRepo
            .Setup(x => x.GetPendingOrdersAsync(It.IsAny<int>()))
            .ReturnsAsync(Result.Ok<IEnumerable<Order>>(new List<Order> { order }));

        _orderRepo
            .Setup(x => x.GetOrderItemsAsync(order.Id))
            .ReturnsAsync(Result.Ok<IEnumerable<OrderItem>>(order.Items));

        _productRepo
            .Setup(x => x.GetProductAsync(It.IsAny<int>()))
            .ReturnsAsync(Result.Ok(smallItem.Product));

        _productRepo
            .Setup(x => x.GetStockQuantityAsync(It.IsAny<int>()))
            .ReturnsAsync(Result.Ok<int?>(200));

        // Act
        var result = await _sut.ProcessCustomerOrders(2);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Single().DiscountPercent.Should().Be(2);
        result.Value.Single().Status.Should().Be("Ready");
    }

    [Fact]
    public async Task ProcessCustomerOrders_UnavailableProducts_SetsOnHoldAndLogsMessage()
    {
        // Arrange
        var order = new Order { Id = 4, Items = new List<OrderItem>() };
        var item = new OrderItem { ProductId = 3, Quantity = 10, Product = new Product { Price = 5000 } };
        order.Items.Add(item);

        _customerRepo
            .Setup(x => x.GetCustomerAsync(It.IsAny<int>()))
            .ReturnsAsync(Result.Ok(new Customer { IsVip = false, RegistrationDate = new DateTime(2024, 1, 1) }));

        _orderRepo
            .Setup(x => x.GetPendingOrdersAsync(It.IsAny<int>()))
            .ReturnsAsync(Result.Ok<IEnumerable<Order>>(new List<Order> { order }));

        _orderRepo
            .Setup(x => x.GetOrderItemsAsync(order.Id))
            .ReturnsAsync(Result.Ok<IEnumerable<OrderItem>>(order.Items));

        _productRepo
            .Setup(x => x.GetProductAsync(It.IsAny<int>()))
            .ReturnsAsync(Result.Ok(item.Product));

        _productRepo
            .Setup(x => x.GetStockQuantityAsync(It.IsAny<int>()))
            .ReturnsAsync(Result.Ok<int?>(5)); // méně než potřeba

        // Act
        var result = await _sut.ProcessCustomerOrders(3);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Single().Status.Should().Be("OnHold");
        _orderLogsRepo.Verify(x => x.SaveLogAsync(order.Id, "Order on hold. Some items are not on stock."), Times.Once);
    }

}
