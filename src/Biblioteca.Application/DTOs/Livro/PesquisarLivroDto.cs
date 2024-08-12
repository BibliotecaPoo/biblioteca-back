namespace Biblioteca.Application.DTOs.Livro;

public class PesquisarLivroDto
{
    public int? Id { get; set; }
    public string? Titulo { get; set; }
    public string? Autor { get; set; }
    public string? Editora { get; set; }
    public string? Categoria { get; set; }
    public int? Codigo { get; set; }
    public bool? Ativo { get; set; }
    public int QuantidadeDeItensPorPagina { get; set; } = 10;
    public int PaginaAtual { get; set; } = 1;
}