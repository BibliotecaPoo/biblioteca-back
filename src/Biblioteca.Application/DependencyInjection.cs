using System.Reflection;
using Biblioteca.Application.Configurations;
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
    public static void ConfigurarCamadaDeAplicacao(this IServiceCollection services, IConfiguration configuration)
    {
        ConfigurarClassesDeConfiguracao(services, configuration);
        ConfigurarDependenciasDeServicos(services);
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.ConfigurarCamadaDeInfraestrutura(configuration);
    }

    private static void ConfigurarClassesDeConfiguracao(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.Configure<StorageSettings>(configuration.GetSection("StorageSettings"));
    }

    private static void ConfigurarDependenciasDeServicos(this IServiceCollection services)
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