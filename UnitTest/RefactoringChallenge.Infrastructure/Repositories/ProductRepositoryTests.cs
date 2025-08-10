using Dapper;
using FluentAssertions;
using Moq;
using Moq.Dapper;
using RefactoringChallenge.Data.Abstractions;
using RefactoringChallenge.Infrastructure.Repositories;
using RefactoringChallenge.Model.Domain;
using System.Data;

namespace RefactoringChallenge.InfrastructureTests.Repositories;

public class ProductRepositoryTests
{
    private readonly Mock<IDbConnection> _connectionMock;
    private readonly Mock<IDatabaseRepository> _databaseRepositoryMock;
    private readonly ProductRepository _sut;

    public ProductRepositoryTests()
    {
        _connectionMock = new Mock<IDbConnection>();
        _databaseRepositoryMock = new Mock<IDatabaseRepository>();
        _databaseRepositoryMock.Setup(x => x.Connection).Returns(_connectionMock.Object);

        _sut = new ProductRepository(_databaseRepositoryMock.Object);
    }

    [Fact]
    public async Task GetProduct_ShouldReturnProduct_WhenProductExists()
    {
        var expectedProduct = new Product { Id = 1, Name = "Laptop", Category = "Electronics", Price = 1200 };

        _connectionMock
            .SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<Product>(
                It.IsAny<string>(),
                It.IsAny<object>(),
                null,
                null,
                null))
            .ReturnsAsync(expectedProduct);

        var result = await _sut.GetProductAsync(1);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expectedProduct);
    }

    [Fact]
    public async Task GetProduct_ShouldFail_WhenProductDoesNotExist()
    {
        _connectionMock
            .SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<Product>(
                It.IsAny<string>(),
                It.IsAny<object>(),
                null,
                null,
                null))
            .ReturnsAsync((Product)null!);

        var result = await _sut.GetProductAsync(99);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "Product not found");
    }

    [Fact]
    public async Task GetStockQuantity_ShouldReturnQuantity()
    {
        int? expectedQuantity = 1;

        _connectionMock
            .SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<int?>(
                It.IsAny<string>(),
                It.IsAny<object>(),
                null,
                null,
                null))
            .ReturnsAsync(expectedQuantity);

        var result = await _sut.GetStockQuantityAsync(2);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedQuantity);
    }

    [Fact]
    public async Task GetStockQuantity_ShouldReturnNull_WhenNoInventoryRow()
    {
        _connectionMock
            .SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<int?>(
                It.IsAny<string>(),
                It.IsAny<object>(),
                null,
                null,
                null))
            .ReturnsAsync((int?)null);

        var result = await _sut.GetStockQuantityAsync(1);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeNull();
    }

    [Fact]
    public async Task UpdateStockQuantity_ShouldReturnOk_WhenUpdateSucceeds()
    {
        _connectionMock
            .SetupDapperAsync(c => c.ExecuteAsync(
                It.IsAny<string>(),
                It.IsAny<object>(),
                null,
                null,
                null))
            .ReturnsAsync(1);

        var result = await _sut.UpdateStockQuantityAsync(1, 5);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateStockQuantity_ShouldFail_WhenNoRowsAffected()
    {
        _connectionMock
            .SetupDapperAsync(c => c.ExecuteAsync(
                It.IsAny<string>(),
                It.IsAny<object>(),
                null,
                null,
                null))
            .ReturnsAsync(0);

        var result = await _sut.UpdateStockQuantityAsync(1, 5);

        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "Update error");
    }
}
