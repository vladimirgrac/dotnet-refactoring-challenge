using RefactoringChallenge.Data.Abstractions;
using RefactoringChallenge.Infrastructure.Abstractions;
using System.Data;
using Dapper;

namespace RefactoringChallenge.Infrastructure.Repositories;
public class OrderLogsRepository(IDatabaseRepository databaseRepository) : IOrderLogsRepository
{
    private readonly IDbConnection _connection = databaseRepository.Connection;

    /// <inheritdoc />
    public async Task SaveLogAsync(int orderId, string message)
    {
        var commandText = "INSERT INTO OrderLogs (OrderId, LogDate, Message) VALUES (@OrderId, @LogDate, @Message)";
        
        await _connection.ExecuteAsync(commandText, new {
            OrderId = orderId,
            LogDate = DateTime.Now,
            Message = message
        });
    }
}
