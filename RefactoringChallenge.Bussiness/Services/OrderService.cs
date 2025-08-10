using FluentResults;
using RefactoringChallenge.Bussiness.Abstractions;
using RefactoringChallenge.Infrastructure.Abstractions;
using RefactoringChallenge.Model.Domain;
using RefactoringChallenge.Model.Extensions;

namespace RefactoringChallenge.Bussiness.Services;

public class OrderService(
    ICustomerRepository customerRepository,
    IOrderRepository orderRepository,
    IOrderLogsRepository orderLogsRepository,
    IProductRepository productRepository) : IOrderService
{
    private readonly ICustomerRepository _customerRepository = customerRepository;
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IOrderLogsRepository _orderLogsRepository = orderLogsRepository;
    private readonly IProductRepository _productRepository = productRepository;

    /// <inheritdoc />
    public async Task<Result<IReadOnlyCollection<Order>>> ProcessCustomerOrders(int customerId)
    {
        if (customerId <= 0)
        {
            return Result.Fail("Invalid customer ID");
        }

        var customerResult = await _customerRepository.GetCustomerAsync(customerId);
        if (customerResult.IsFailed)
        {
            return customerResult.ToResult();
        }

        var customer = customerResult.Value;

        var pendingOrdersResult = await _orderRepository.GetPendingOrdersAsync(customerId);
        if (pendingOrdersResult.IsFailed)
        {
            return pendingOrdersResult.ToResult();
        }

        var pendingOrders = pendingOrdersResult.Value;
        var processedOrders = new List<Order>();

        foreach (var order in pendingOrders)
        {
            // fill items 
            var orderItemsResult = await _orderRepository.GetOrderItemsAsync(order.Id);
            order.Items.AddRange([.. orderItemsResult.Value]);

            // fill products
            foreach (var orderItem in order.Items)
            {
                var productResult = await _productRepository.GetProductAsync(orderItem.ProductId);
                if (productResult.IsFailed)
                {
                    continue;
                }

                orderItem.Product = productResult.Value;
            }

            // recalculate order
            order.ProcessOrder(customer.IsVip, customer.RegistrationDate.Year);

            // update order
            await _orderRepository.UpdateOrderAsync(order);

            bool allProductsAvailable = true;

            // process inventory
            foreach (var item in order.Items)
            {
                var stockQuantityResult = await _productRepository.GetStockQuantityAsync(item.ProductId);
                if (stockQuantityResult.IsFailed) { continue; }

                var stockQuantity = stockQuantityResult.Value;
                if (stockQuantity == null || stockQuantity < item.Quantity)
                {
                    allProductsAvailable = false;
                    break;
                }
            }

            if (allProductsAvailable)
            {
                // update inventory
                foreach (var item in order.Items)
                {
                    await _productRepository.UpdateStockQuantityAsync(item.ProductId, item.Quantity);
                }

                // update order status
                order.Status = "Ready";

                await UpdateOrderStatus(order.Id, order.Status, $"Order completed with {order.DiscountPercent}% discount. Total price: {order.TotalAmount}");
            }
            else
            {
                // update order status
                order.Status = "OnHold";

                await UpdateOrderStatus(order.Id, order.Status, "Order on hold. Some items are not on stock.");
            }

            processedOrders.Add(order);
        }



        return processedOrders;
    }

    private async Task UpdateOrderStatus(int orderId, string status, string message)
    {
        await _orderRepository.UpdateOrderStatusAsync(orderId, status);

        // log
        await _orderLogsRepository.SaveLogAsync(orderId, message);
    }
}
