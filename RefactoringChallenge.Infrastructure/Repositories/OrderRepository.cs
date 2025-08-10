using Dapper;
using FluentResults;
using RefactoringChallenge.Data.Abstractions;
using RefactoringChallenge.Infrastructure.Abstractions;
using RefactoringChallenge.Model.Domain;
using System.Data;

namespace RefactoringChallenge.Infrastructure.Repositories;
public class OrderRepository(IDatabaseRepository databaseRepository) : IOrderRepository
{
    private readonly IDbConnection _connection = databaseRepository.Connection;

    /// <inheritdoc />
    public async Task<Result<IEnumerable<Order>>> GetPendingOrdersAsync(int customerId)
    {
        var commandText = "SELECT Id, CustomerId, OrderDate, TotalAmount, Status FROM Orders WHERE CustomerId = @CustomerId AND Status = 'Pending'";
        var result = await _connection.QueryAsync<Order>(commandText, new
        {
            CustomerId = customerId
        });

        return result is null || !result.Any()
            ? Result.Fail("Orders not found")
            : Result.Ok(result);
    }

    /// <inheritdoc />
    public async Task<Result<IEnumerable<OrderItem>>> GetOrderItemsAsync(int orderId)
    {
        var commandText = "SELECT Id, OrderId, ProductId, Quantity, UnitPrice FROM OrderItems WHERE OrderId = @OrderId";

        var result = await _connection.QueryAsync<OrderItem>(commandText, new
        {
            OrderId = orderId
        });

        return result is null || !result.Any()
            ? Result.Fail("OrderItems not found")
            : Result.Ok(result);
    }

    /// <inheritdoc />
    public async Task<Result> UpdateOrderAsync(Order order)
    {
        var commandText = "UPDATE Orders SET TotalAmount = @TotalAmount, DiscountPercent = @DiscountPercent, DiscountAmount = @DiscountAmount, Status = @Status WHERE Id = @OrderId";

        var result = await _connection.ExecuteAsync(commandText, new
        {
            TotalAmount = order.TotalAmount,
            DiscountPercent = order.DiscountPercent,
            DiscountAmount = order.DiscountAmount,
            Status = order.Status,
            OrderId = order.Id
        });

        return result <= 0
            ? Result.Fail("Update error")
            : Result.Ok();
    }

    /// <inheritdoc />
    public async Task<Result> UpdateOrderStatusAsync(int orderId, string status)
    {
        var commandText = "UPDATE Orders SET Status = @Status WHERE Id = @OrderId";

        var result = await _connection.ExecuteAsync(commandText, new
        {
            Status = status,
            OrderId = orderId
        });

        return result <= 0
            ? Result.Fail("Update error")
            : Result.Ok();
    }
}
