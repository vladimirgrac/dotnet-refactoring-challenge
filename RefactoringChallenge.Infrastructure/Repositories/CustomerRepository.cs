using Dapper;
using FluentResults;
using RefactoringChallenge.Data.Abstractions;
using RefactoringChallenge.Infrastructure.Abstractions;
using RefactoringChallenge.Model.Domain;
using System.Data;

namespace RefactoringChallenge.Infrastructure.Repositories;
public class CustomerRepository(IDatabaseRepository databaseRepository) : ICustomerRepository
{
    private readonly IDbConnection _connection = databaseRepository.Connection;

    /// <inheritdoc />
    public async Task<Result<Customer>> GetCustomerAsync(int customerId)
    {
        var commandText = "SELECT Id, Name, Email, IsVip, RegistrationDate FROM Customers WHERE Id = @CustomerId";
        var result = await _connection.QueryFirstOrDefaultAsync<Customer>(commandText, new
        {
            CustomerId = customerId
        });

        return result is null 
            ? Result.Fail("Customer was not found") 
            : Result.Ok(result);
    }
}
