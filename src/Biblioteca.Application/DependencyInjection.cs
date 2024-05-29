using System.Reflection;
using Biblioteca.Application.Configuration;
using Biblioteca.Application.Contracts.Services;
using Biblioteca.Application.Notifications;
using Biblioteca.Application.Services;
using Biblioteca.Domain.Entities;
using Biblioteca.Infra.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScottBrady91.AspNetCore.Identity;

namespace Biblioteca.Application;

public static class DependencyInjection
{
    public static void SetupSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.Configure<StorageSettings>(configuration.GetSection("StorageSettings"));
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
            .AddScoped<IPasswordHasher<Administrador>, Argon2PasswordHasher<Administrador>>()
            .AddScoped<IPasswordHasher<Usuario>, Argon2PasswordHasher<Usuario>>()
            .AddScoped<INotificator, Notificator>();

        services
            .AddScoped<IAuthService, AuthService>()
            .AddScoped<IUsuarioService, UsuarioService>()
            .AddScoped<ILivroService, LivroService>()
            .AddScoped<IEmprestimoService, EmprestimoService>();
    }
}