using Microsoft.Extensions.DependencyInjection;
using RefactoringChallenge.Infrastructure.Abstractions;
using RefactoringChallenge.Infrastructure.Repositories;

namespace RefactoringChallenge.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IOrderLogsRepository, OrderLogsRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();

        return services;
    }
}