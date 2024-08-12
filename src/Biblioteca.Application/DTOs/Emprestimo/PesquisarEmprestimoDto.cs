namespace Biblioteca.Application.DTOs.Emprestimo;

public class PesquisarEmprestimoDto
{
    public int? Id { get; set; }
    public int? UsuarioId { get; set; }
    public string? UsuarioMatricula { get; set; }
    public int? LivroId { get; set; }
    public int? LivroCodigo { get; set; }
    public bool? Ativo { get; set; }
    public int QuantidadeDeItensPorPagina { get; set; } = 10;
    public int PaginaAtual { get; set; } = 1;
}