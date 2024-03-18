using Biblioteca.Core.Enum;
using Biblioteca.Core.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace Biblioteca.API.Configurations;

public static class AuthenticationConfiguration
{
    public static void AddAuthenticationConfig(this IServiceCollection services, IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        var appSettingsSection = configuration.GetSection("JwtSettings");
        services.Configure<JwtSettings>(appSettingsSection);

        var appSettings = appSettingsSection.Get<JwtSettings>();
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.IncludeErrorDetails = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = appSettings?.Emissor ?? string.Empty,
                    ValidAudiences = appSettings?.Audiences()
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(ETipoUsuario.Administrador.ToString(), builder =>
            {
                builder
                    .RequireAuthenticatedUser()
                    .RequireClaim("TipoUsuario", ETipoUsuario.Administrador.ToString());
            });

            options.AddPolicy(ETipoUsuario.Comum.ToString(), builder =>
            {
                builder
                    .RequireAuthenticatedUser()
                    .RequireClaim("TipoUsuario", ETipoUsuario.Comum.ToString());
            });
        });

        var redisConnection = configuration.GetConnectionString("RedisConnection");
        if (!string.IsNullOrEmpty(redisConnection))
        {
            services
                .AddDataProtection()
                .PersistKeysToStackExchangeRedis(ConnectionMultiplexer.Connect(redisConnection),
                    $"Biblioteca-{environment.EnvironmentName}-DataProtection-Keys-");
        }
        else
        {
            services
                .AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(appSettings.CaminhoKeys));
        }

        services
            .AddJwksManager()
            .UseJwtValidation();

        services.AddMemoryCache();
        services.AddHttpContextAccessor();
    }

    public static void UseAuthenticationConfig(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}