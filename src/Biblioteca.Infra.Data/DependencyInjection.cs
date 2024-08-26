using Biblioteca.Domain.Contracts.Repositories;
using Biblioteca.Infra.Data.Context;
using Biblioteca.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Biblioteca.Infra.Data;

public static class DependencyInjection
{
    public static void ConfigurarCamadaDeInfraestrutura(this IServiceCollection services, IConfiguration configuration)
    {
        ConfigurarBancoDeDados(services, configuration);
        ConfigurarDependenciasDeRepositorios(services);
    }

    private static void ConfigurarBancoDeDados(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var serverVersion = ServerVersion.AutoDetect(connectionString);

            options.UseMySql(connectionString, serverVersion);
            options.EnableDetailedErrors();
            options.EnableSensitiveDataLogging();
        });
    }

    private static void ConfigurarDependenciasDeRepositorios(this IServiceCollection services)
    {
        services
            .AddScoped<IAdministradorRepository, AdministradorRepository>()
            .AddScoped<IUsuarioRepository, UsuarioRepository>()
            .AddScoped<ILivroRepository, LivroRepository>()
            .AddScoped<IEmprestimoRepository, EmprestimoRepository>();
    }
}