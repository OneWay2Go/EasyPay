using EasyPay.Application.Interfaces;
using EasyPay.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EasyPay.Application.Exceptions;

public static class ServiceDI
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        return services;
    }
}