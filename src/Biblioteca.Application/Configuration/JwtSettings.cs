namespace Biblioteca.Application.Configuration;

public class JwtSettings
{
    public int ExpiracaoHoras { get; set; }
    public string CaminhoKeys { get; set; } = null!;
}