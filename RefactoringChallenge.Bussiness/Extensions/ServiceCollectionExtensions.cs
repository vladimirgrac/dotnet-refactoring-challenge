using Microsoft.Extensions.DependencyInjection;
using RefactoringChallenge.Bussiness.Abstractions;
using RefactoringChallenge.Bussiness.Services;

namespace RefactoringChallenge.Bussiness.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBussiness(this IServiceCollection services)
    {
        services.AddScoped<IOrderService, OrderService>();

        return services;
    }
}
