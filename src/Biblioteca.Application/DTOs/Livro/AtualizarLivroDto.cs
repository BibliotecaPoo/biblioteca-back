namespace Biblioteca.Application.DTOs.Livro;

public class AtualizarLivroDto
{
    public int Id { get; set; }
    public string? Titulo { get; set; }
    public string? Autor { get; set; }
    public string? Edicao { get; set; }
    public string? Editora { get; set; }
    public string? Categoria { get; set; }
    public int? AnoPublicacao { get; set; }
    public int? QuantidadeExemplaresDisponiveisEmEstoque { get; set; }
}