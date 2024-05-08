using Biblioteca.Domain.Enums;

namespace Biblioteca.Domain.Entities;

public class Emprestimo : Entity
{
    public DateTime DataEmprestimo { get; set; }
    public DateTime DataDevolucaoPrevista { get; set; }
    public DateTime? DataDevolucaoRealizada { get; set; }
    public EStatusEmprestimo StatusEmprestimo { get; set; }
    public int IdUsuario { get; set; }
    public int IdLivro { get; set; }

    // Relation
    public Usuario Usuario { get; set; } = null!;
    public Livro Livro { get; set; } = null!;
}