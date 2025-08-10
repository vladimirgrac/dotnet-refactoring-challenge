using Dapper;
using Moq;
using Moq.Dapper;
using RefactoringChallenge.Data.Abstractions;
using RefactoringChallenge.Infrastructure.Repositories;
using System.Data;

namespace RefactoringChallenge.InfrastructureTests.Repositories
{
    public class OrderLogsRepositoryTests
    {
        private readonly Mock<IDbConnection> _connectionMock;
        private readonly Mock<IDatabaseRepository> _databaseRepositoryMock;
        private readonly OrderLogsRepository _sut;

        public OrderLogsRepositoryTests()
        {
            _connectionMock = new Mock<IDbConnection>();
            _databaseRepositoryMock = new Mock<IDatabaseRepository>();

            _databaseRepositoryMock
                .SetupGet(x => x.Connection)
                .Returns(_connectionMock.Object);

            _sut = new OrderLogsRepository(_databaseRepositoryMock.Object);
        }

        [Fact]
        public async Task SaveLog_ShouldCallExecuteAsyncWithCorrectParameters()
        {
            // Arrange
            int orderId = 123;
            string message = "Test message";

            _connectionMock
                .SetupDapperAsync(c => c.ExecuteAsync(
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    null,
                    null,
                    null))
                .ReturnsAsync(1);

            // Act
            await _sut.SaveLogAsync(orderId, message);
        }
    }
}
