using Dapper;
using FluentAssertions;
using Moq;
using Moq.Dapper;
using RefactoringChallenge.Data.Abstractions;
using RefactoringChallenge.Infrastructure.Abstractions;
using RefactoringChallenge.Infrastructure.Repositories;
using RefactoringChallenge.Model.Domain;
using System.Data;

namespace RefactoringChallenge.InfrastructureTests.Repositories;

public class CustomerRepositoryTests
{
    private readonly Mock<IDatabaseRepository> _databaseRepositoryMock;
    private readonly Mock<IDbConnection> _connectionMock;
    private readonly ICustomerRepository _sut;

    public CustomerRepositoryTests()
    {
        _databaseRepositoryMock = new Mock<IDatabaseRepository>();
        _connectionMock = new Mock<IDbConnection>();

        _databaseRepositoryMock
            .Setup(x => x.Connection)
            .Returns(_connectionMock.Object);

        _sut = new CustomerRepository(_databaseRepositoryMock.Object);
    }

    [Fact]
    public async Task GetCustomer_WhenCustomerExists_ShouldReturnOkResult()
    {
        // Arrange
        var expectedCustomer = new Customer
        {
            Id = 1,
            Name = "John Doe",
            Email = "john@example.com",
            IsVip = true,
            RegistrationDate = new DateTime(2020, 1, 1)
        };

        _connectionMock
            .SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<Customer>(
                It.IsAny<string>(),
                It.IsAny<object>(),
                null,
                null,
                null))
            .ReturnsAsync(expectedCustomer);

        // Act
        var result = await _sut.GetCustomerAsync(1);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expectedCustomer);
    }

    [Fact]
    public async Task GetCustomer_WhenCustomerDoesNotExist_ShouldReturnFailResult()
    {
        // Arrange
        _connectionMock
            .SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<Customer>(
                It.IsAny<string>(),
                It.IsAny<object>(),
                null,
                null,
                null))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await _sut.GetCustomerAsync(42);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "Customer was not found");
    }
}
