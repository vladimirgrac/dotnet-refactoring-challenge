using FluentResults;
using RefactoringChallenge.Model.Domain;

namespace RefactoringChallenge.Infrastructure.Abstractions;
public interface ICustomerRepository
{
    /// <summary>
    /// Get customer by ID
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    Task<Result<Customer>> GetCustomerAsync(int customerId);
}
