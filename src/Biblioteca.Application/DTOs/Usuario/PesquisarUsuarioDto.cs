namespace Biblioteca.Application.DTOs.Usuario;

public class PesquisarUsuarioDto
{
    public int? Id { get; set; }
    public string? Nome { get; set; }
    public string? Email { get; set; }
    public string? Matricula { get; set; }
    public string? Curso { get; set; }
    public int QuantidadeDeItensPorPagina { get; set; } = 10;
    public int PaginaAtual { get; set; } = 1;
}