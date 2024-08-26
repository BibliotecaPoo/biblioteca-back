using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Biblioteca.API.Configurations.Swagger;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
        }
    }

    private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var contact = new OpenApiContact
        {
            Name = "Guilherme Silveira",
            Email = "guilhermesilveirasousa@gmail.com",
            Url = new Uri("https://github.com/guilheermesilveira")
        };

        var license = new OpenApiLicense
        {
            Name = "Free License",
            Url = new Uri("https://github.com/guilheermesilveira")
        };

        var info = new OpenApiInfo
        {
            Title = "Biblioteca Acadêmica API",
            Version = description.ApiVersion.ToString(),
            Contact = contact,
            License = license
        };

        if (description.IsDeprecated)
        {
            info.Description += "Esta versão da API foi descontinuada.";
        }

        return info;
    }
}