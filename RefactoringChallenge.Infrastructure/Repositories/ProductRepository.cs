using Dapper;
using FluentResults;
using RefactoringChallenge.Data.Abstractions;
using RefactoringChallenge.Infrastructure.Abstractions;
using RefactoringChallenge.Model.Domain;
using System.Data;

namespace RefactoringChallenge.Infrastructure.Repositories;
public class ProductRepository(IDatabaseRepository databaseRepository) : IProductRepository
{
    private readonly IDbConnection _connection = databaseRepository.Connection;

    /// <inheritdoc />
    public async Task<Result<Product>> GetProductAsync(int productId)
    {
        var queryText = "SELECT Id, Name, Category, Price FROM Products WHERE Id = @ProductId";
        var result = await _connection.QueryFirstOrDefaultAsync<Product>(queryText, new
        {
            ProductId = productId
        });

        return result is null
            ? Result.Fail("Product not found")
            : Result.Ok(result);
    }

    /// <inheritdoc />
    public async Task<Result<int?>> GetStockQuantityAsync(int productId)
    {
        var queryText = "SELECT StockQuantity FROM Inventory WHERE ProductId = @ProductId";
        var result = await _connection.QueryFirstOrDefaultAsync<int?>(queryText, new
        {
            ProductId = productId
        });

        return Result.Ok(result);
    }

    /// <inheritdoc />
    public async Task<Result> UpdateStockQuantityAsync(int productId, int stockQuantity)
    {
        var commandText = "UPDATE Inventory SET StockQuantity = StockQuantity - @Quantity WHERE ProductId = @ProductId";
        var result = await _connection.ExecuteAsync(commandText, new
        {
            Quantity = stockQuantity,
            ProductId = productId
        });

        return result <= 0
            ? Result.Fail("Update error")
            : Result.Ok();
    }
}
