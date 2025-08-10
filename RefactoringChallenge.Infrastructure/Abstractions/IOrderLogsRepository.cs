namespace RefactoringChallenge.Infrastructure.Abstractions;

public interface IOrderLogsRepository
{
    /// <summary>
    /// Save log message for given order ID
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    Task SaveLogAsync(int orderId, string message);
}
