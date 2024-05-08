namespace Biblioteca.Domain.Entities;

public class Usuario : Pessoa
{
    public string Matricula { get; set; } = null!;
    public bool Bloqueado { get; set; }
    public int? DiasBloqueado { get; set; }
    public DateTime? DataInicioBloqueio { get; set; }
    public DateTime? DataFimBloqueio { get; set; }

    // Relation
    public virtual List<Emprestimo> Emprestimos { get; set; } = new();
}