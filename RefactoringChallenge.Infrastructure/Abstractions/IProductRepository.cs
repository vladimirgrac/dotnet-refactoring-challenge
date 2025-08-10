using FluentResults;
using RefactoringChallenge.Model.Domain;

namespace RefactoringChallenge.Infrastructure.Abstractions;
public interface IProductRepository
{
    /// <summary>
    /// Get product by ID
    /// </summary>
    /// <param name="productId"></param>
    /// <returns></returns>
    Task<Result<Product>> GetProductAsync(int productId);
    /// <summary>
    /// Get product stock quantity
    /// </summary>
    /// <param name="productId"></param>
    /// <returns></returns>
    Task<Result<int?>> GetStockQuantityAsync(int productId);
    /// <summary>
    /// Update product stock quantity
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="stockQuantity"></param>
    /// <returns></returns>
    Task<Result> UpdateStockQuantityAsync(int productId, int stockQuantity);
}
