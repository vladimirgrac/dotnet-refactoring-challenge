using FluentResults;
using RefactoringChallenge.Model.Domain;

namespace RefactoringChallenge.Bussiness.Abstractions;

public interface IOrderService
{
    /// <summary>
    /// Process customers pending orders
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    Task<Result<IReadOnlyCollection<Order>>> ProcessCustomerOrders(int customerId);
}
