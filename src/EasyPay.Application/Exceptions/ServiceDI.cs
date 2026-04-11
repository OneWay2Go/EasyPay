using EasyPay.Application.Interfaces;
using EasyPay.Application.Services;
using EasyPay.Application.Services.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;

namespace EasyPay.Application.Exceptions;

public static class ServiceDI
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IGameService, GameService>();

        services.AddRouting(options =>
        {
            options.LowercaseUrls = true;
        });
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o => o.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidIssuer = ""
            });
        
        return services;
    }
}