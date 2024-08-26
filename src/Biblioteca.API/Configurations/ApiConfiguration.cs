using System.Globalization;
using System.Net;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Newtonsoft.Json;

namespace Biblioteca.API.Configurations;

public static class ApiConfiguration
{
    public static void AdicionarConfiguracaoDaApi(this IServiceCollection services, IConfigurationBuilder builder,
        IHostEnvironment environment)
    {
        ConfigurarAmbiente(builder, environment);
        ConfigurarRotas(services);
        ConfigurarConversoresJson(services);
        ConfigurarLocalizacao(services);
        ConfigurarCors(services);
        ConfigurarComportamentoDaApi(services);
        ConfigurarVersionamentoDaApi(services);
        ConfigurarRespostaDeCache(services);
    }

    public static void UsarConfiguracaoDaApi(this IApplicationBuilder app)
    {
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        app.UseCors("default");
    }

    private static void ConfigurarAmbiente(this IConfigurationBuilder builder, IHostEnvironment environment)
    {
        builder
            .SetBasePath(environment.ContentRootPath)
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", true, true)
            .AddEnvironmentVariables();
    }

    private static void ConfigurarRotas(this IServiceCollection services)
    {
        services.AddRouting(options => options.LowercaseUrls = true);
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
        services.AddControllers(config =>
        {
            config.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
        });
    }

    private static void ConfigurarConversoresJson(this IServiceCollection services)
    {
        services.AddDateOnlyTimeOnlyStringConverters();
        services.AddControllers()
            .AddDataAnnotationsLocalization()
            .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.MaxDepth = 3;
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
    }

    private static void ConfigurarLocalizacao(this IServiceCollection services)
    {
        services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[] { new CultureInfo("pt-BR") };
            options.DefaultRequestCulture = new RequestCulture("pt-BR", "pt-BR");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
        });
    }

    private static void ConfigurarCors(this IServiceCollection services)
    {
        services.AddCors(corsOptions =>
        {
            corsOptions.AddPolicy("default", policy =>
            {
                policy
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
    }

    private static void ConfigurarComportamentoDaApi(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context => new BadRequestObjectResult(new
            {
                Title = "Model inválida!",
                Status = (int)HttpStatusCode.BadRequest,
                Erros = context.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage)
            });
        });
    }

    private static void ConfigurarVersionamentoDaApi(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
            })
            .AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
    }

    private static void ConfigurarRespostaDeCache(this IServiceCollection services)
    {
        services.AddResponseCaching();
    }

    private sealed class SlugifyParameterTransformer : IOutboundParameterTransformer
    {
        public string? TransformOutbound(object? value)
        {
            return value == null
                ? null
                : Regex.Replace(value.ToString() ?? string.Empty, "([a-z])([A-Z])", "$1-$2").ToLower();
        }
    }
}