using RefactoringChallenge.Data.Abstractions;
using System.Data;

namespace RefactoringChallenge.Data.Repository;
public class DatabaseRepository(IDbConnectionFactory connectionFactory) : IDatabaseRepository
{
    public IDbConnection Connection { get; } = connectionFactory.CreateConnection();

    public void Dispose() => Connection.Dispose();
}
