using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using RefactoringChallenge.Data.Abstractions;
using RefactoringChallenge.Data.Configuration;
using System.Data;

namespace RefactoringChallenge.Data.Repository;
internal class DbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(IOptions<DatabaseOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options.Value.ConnectionString);

        _connectionString = options.Value.ConnectionString;
    }

    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}
