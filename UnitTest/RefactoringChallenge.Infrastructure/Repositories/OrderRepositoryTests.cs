using Dapper;
using FluentAssertions;
using Moq;
using Moq.Dapper;
using RefactoringChallenge.Data.Abstractions;
using RefactoringChallenge.Infrastructure.Repositories;
using RefactoringChallenge.InfrastructureTests.Repositories;
using RefactoringChallenge.Model.Domain;
using System.Data;

namespace RefactoringChallenge.InfrastructureTests.Repositories;

public class OrderRepositoryTests
{
    private readonly Mock<IDbConnection> _connectionMock;
    private readonly Mock<IDatabaseRepository> _databaseRepositoryMock;
    private readonly OrderRepository _sut;

    public OrderRepositoryTests()
    {
        _connectionMock = new Mock<IDbConnection>();
        _databaseRepositoryMock = new Mock<IDatabaseRepository>();
        _databaseRepositoryMock.Setup(x => x.Connection).Returns(_connectionMock.Object);

        _sut = new OrderRepository(_databaseRepositoryMock.Object);
    }

    [Fact]
    public async Task GetPendingOrdersAsync_ShouldReturnOrders_WhenOrdersExist()
    {
        // Arrange
        var orders = new List<Order>
        {
            new Order { Id = 1, CustomerId = 123, Status = "Pending" }
        };

        _connectionMock
            .SetupDapperAsync(c => c.QueryAsync<Order>(
                It.IsAny<string>(),
                It.IsAny<object>(),
                null,
                null,
                null))
            .ReturnsAsync(orders);

        // Act
        var result = await _sut.GetPendingOrdersAsync(123);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(orders);
    }

    [Fact]
    public async Task GetPendingOrdersAsync_ShouldFail_WhenNoOrdersExist()
    {
        // Arrange
        _connectionMock
            .SetupDapperAsync(c => c.QueryAsync<Order>(
                It.IsAny<string>(),
                It.IsAny<object>(),
                null,
                null,
                null))
            .ReturnsAsync(Enumerable.Empty<Order>());

        // Act
        var result = await _sut.GetPendingOrdersAsync(123);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "Orders not found");
    }

    [Fact]
    public async Task GetOrderItemsAsync_ShouldReturnItems_WhenItemsExist()
    {
        // Arrange
        var items = new List<OrderItem>
        {
            new OrderItem { Id = 1, OrderId = 456, ProductId = 2, Quantity = 3, UnitPrice = 50 }
        };

        _connectionMock
            .SetupDapperAsync(c => c.QueryAsync<OrderItem>(
                It.IsAny<string>(),
                It.IsAny<object>(),
                null,
                null,
                null))
            .ReturnsAsync(items);

        // Act
        var result = await _sut.GetOrderItemsAsync(456);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(items);
    }

    [Fact]
    public async Task GetOrderItemsAsync_ShouldFail_WhenNoItemsExist()
    {
        // Arrange
        _connectionMock
            .SetupDapperAsync(c => c.QueryAsync<OrderItem>(
                It.IsAny<string>(),
                It.IsAny<object>(),
                null,
                null,
                null))
            .ReturnsAsync(Enumerable.Empty<OrderItem>());

        // Act
        var result = await _sut.GetOrderItemsAsync(456);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "OrderItems not found");
    }

    [Fact]
    public async Task UpdateOrderAsync_ShouldReturnOk_WhenUpdateSucceeds()
    {
        // Arrange
        var order = new Order
        {
            Id = 10,
            TotalAmount = 100,
            DiscountPercent = 5,
            DiscountAmount = 5,
            Status = "Processed"
        };

        _connectionMock
            .SetupDapperAsync(c => c.ExecuteAsync(
                It.IsAny<string>(),
                It.IsAny<object>(),
                null,
                null,
                null))
            .ReturnsAsync(1);

        // Act
        var result = await _sut.UpdateOrderAsync(order);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateOrderAsync_ShouldFail_WhenNoRowsAffected()
    {
        // Arrange
        var order = new Order { Id = 10 };

        _connectionMock
            .SetupDapperAsync(c => c.ExecuteAsync(
                It.IsAny<string>(),
                It.IsAny<object>(),
                null,
                null,
                null))
            .ReturnsAsync(0);

        // Act
        var result = await _sut.UpdateOrderAsync(order);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "Update error");
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_ShouldReturnOk_WhenUpdateSucceeds()
    {
        // Arrange
        _connectionMock
            .SetupDapperAsync(c => c.ExecuteAsync(
                It.IsAny<string>(),
                It.IsAny<object>(),
                null,
                null,
                null))
            .ReturnsAsync(1);

        // Act
        var result = await _sut.UpdateOrderStatusAsync(10, "Ready");

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_ShouldFail_WhenNoRowsAffected()
    {
        // Arrange
        _connectionMock
            .SetupDapperAsync(c => c.ExecuteAsync(
                It.IsAny<string>(),
                It.IsAny<object>(),
                null,
                null,
                null))
            .ReturnsAsync(0);

        // Act
        var result = await _sut.UpdateOrderStatusAsync(10, "Ready");

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "Update error");
    }
}
