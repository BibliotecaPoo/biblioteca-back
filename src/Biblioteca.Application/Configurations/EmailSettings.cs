namespace Biblioteca.Application.Configurations;

public class EmailSettings
{
    public string Nome { get; set; } = String.Empty;
    public string Usuario { get; set; } = String.Empty;
    public string Senha { get; set; } = String.Empty;
    public string Servidor { get; set; } = String.Empty;
    public int Porta { get; set; }
}