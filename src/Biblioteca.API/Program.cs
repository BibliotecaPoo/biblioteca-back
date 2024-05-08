using System.Globalization;
using Biblioteca.API.Configurations;
using Biblioteca.Application;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services
    .Configure<RequestLocalizationOptions>(o =>
    {
        var supportedCultures = new[] { new CultureInfo("pt-BR") };
        o.DefaultRequestCulture = new RequestCulture("pt-BR", "pt-BR");
        o.SupportedCultures = supportedCultures;
        o.SupportedUICultures = supportedCultures;
    });

builder
    .Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", true, true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
    .AddEnvironmentVariables();

builder
    .Services
    .AddAuthenticationConfig(builder.Configuration);

builder
    .Services
    .AddApiConfiguration();

builder
    .Services
    .AddVersioning();

builder
    .Services
    .AddSwagger();

builder
    .Services
    .SetupSettings(builder.Configuration);

builder
    .Services
    .ConfigureApplication(builder.Configuration);

builder
    .Services
    .AddServices();

var app = builder.Build();

app.UseApiConfiguration();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthenticationConfig();

app.MapControllers();

app.Run();