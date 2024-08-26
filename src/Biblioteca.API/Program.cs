using Biblioteca.API.Configurations;
using Biblioteca.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AdicionarConfiguracaoDaApi(builder.Configuration, builder.Environment);

builder.Services.ConfigurarCamadaDeAplicacao(builder.Configuration);

builder.Services.AdicionarConfiguracaoDeAutenticacao(builder.Configuration);

builder.Services.AdicionarConfiguracaoDoSwagger();

var app = builder.Build();

app.UsarConfiguracaoDaApi();

app.UsarConfiguracaoDeAutenticacao();

app.UsarConfiguracaoDoSwagger();

app.MapControllers();

app.Run();