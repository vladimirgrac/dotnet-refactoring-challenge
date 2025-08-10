using Microsoft.Extensions.DependencyInjection;
using RefactoringChallenge.Data.Abstractions;
using RefactoringChallenge.Data.Repository;

namespace RefactoringChallenge.Data.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
        services.AddTransient<IDatabaseRepository, DatabaseRepository>();

        return services;
    }
}
