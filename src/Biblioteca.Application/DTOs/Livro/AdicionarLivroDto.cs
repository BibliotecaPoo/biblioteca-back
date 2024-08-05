namespace Biblioteca.Application.DTOs.Livro;

public class AdicionarLivroDto
{
    public string Titulo { get; set; } = null!;
    public string Autor { get; set; } = null!;
    public string Edicao { get; set; } = null!;
    public string Editora { get; set; } = null!;
    public string Categoria { get; set; } = null!;
    public int AnoPublicacao { get; set; }
    public int QuantidadeExemplaresDisponiveisEmEstoque { get; set; }
}