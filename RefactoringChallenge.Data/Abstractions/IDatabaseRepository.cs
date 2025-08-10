using System.Data;

namespace RefactoringChallenge.Data.Abstractions;
public interface IDatabaseRepository : IDisposable
{
    IDbConnection Connection { get; }
}
