using Biblioteca.Domain.Enums;

namespace Biblioteca.Domain.Entities;

public class Livro : Entity
{
    public string Titulo { get; set; } = null!;
    public string Autor { get; set; } = null!;
    public string Edicao { get; set; } = null!;
    public string Editora { get; set; } = null!;
    public int AnoPublicacao { get; set; }
    public int QuantidadeExemplaresDisponiveisEmEstoque { get; set; }
    public int QuantidadeExemplaresDisponiveisParaEmprestimo { get; set; }
    public EStatusLivro StatusLivro { get; set; }
    public string? NomeArquivoCapa { get; set; }

    // Relation
    public virtual List<Emprestimo> Emprestimos { get; set; } = new();
}