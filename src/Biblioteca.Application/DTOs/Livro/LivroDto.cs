namespace Biblioteca.Application.DTOs.Livro;

public class LivroDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = null!;
    public string Autor { get; set; } = null!;
    public string Edicao { get; set; } = null!;
    public string Editora { get; set; } = null!;
    public string Categoria { get; set; } = null!;
    public int Codigo { get; set; }
    public int AnoPublicacao { get; set; }
    public int QuantidadeExemplaresDisponiveisEmEstoque { get; set; }
    public int QuantidadeExemplaresDisponiveisParaEmprestimo { get; set; }
    public string StatusLivro { get; set; } = null!;
    public string? NomeArquivoCapa { get; set; }
    public bool Ativo { get; set; }
}