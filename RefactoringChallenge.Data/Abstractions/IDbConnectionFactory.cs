using System.Data;

namespace RefactoringChallenge.Data.Abstractions;
public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
    
}
