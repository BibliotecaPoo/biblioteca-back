using Biblioteca.Domain.Enums;

namespace Biblioteca.Application.DTOs.Livro;

public class LivroDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = null!;
    public string Autor { get; set; } = null!;
    public string Edicao { get; set; } = null!;
    public string Editora { get; set; } = null!;
    public int AnoPublicacao { get; set; }
    public int QuantidadeExemplares { get; set; }
    public int QuantidadeExemplaresDisponiveisParaEmprestimo { get; set; }
    public EStatusLivro StatusLivro { get; set; }
    public string? CaminhoCapa { get; set; }
}