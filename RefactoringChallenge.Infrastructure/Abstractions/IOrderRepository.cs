using FluentResults;
using RefactoringChallenge.Model.Domain;

namespace RefactoringChallenge.Infrastructure.Abstractions;

public interface IOrderRepository
{
    /// <summary>
    /// Get pending orders
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    Task<Result<IEnumerable<Order>>> GetPendingOrdersAsync(int customerId);
    /// <summary>
    /// Get order items
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    Task<Result<IEnumerable<OrderItem>>> GetOrderItemsAsync(int orderId);
    /// <summary>
    /// Update order
    /// </summary>
    /// <param name="order"></param>
    /// <returns></returns>
    Task<Result> UpdateOrderAsync(Order order);
    /// <summary>
    /// Update order status
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    Task<Result> UpdateOrderStatusAsync(int orderId, string status);
}
