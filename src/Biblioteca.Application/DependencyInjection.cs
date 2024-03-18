using System.Reflection;
using Biblioteca.Application.Contracts.Services;
using Biblioteca.Application.Notifications;
using Biblioteca.Application.Services;
using Biblioteca.Core.Settings;
using Biblioteca.Domain.Entities;
using Biblioteca.Infra.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScottBrady91.AspNetCore.Identity;

namespace Biblioteca.Application;

public static class DependencyInjection
{
    public static void SetupSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
    }

    public static void ConfigureApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.ConfigureDbContext(configuration);
        services.RepositoryDependency();
    }

    public static void AddServices(this IServiceCollection services)
    {
        services
            .AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services
            .AddScoped<IPasswordHasher<Usuario>, Argon2PasswordHasher<Usuario>>()
            .AddScoped<INotificator, Notificator>();

        services
            .AddScoped<IAuthService, AuthService>()
            .AddScoped<IUsuarioService, UsuarioService>();
    }
}