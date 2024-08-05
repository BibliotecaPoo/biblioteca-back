namespace Biblioteca.Domain.Entities;

public class Usuario : Pessoa
{
    public string Matricula { get; set; } = null!;
    public string Curso { get; set; } = null!;
    public int? QuantidadeEmprestimosPermitida { get; set; }
    public int? QuantidadeEmprestimosRealizados { get; set; }
    public bool? Bloqueado { get; set; }

    // Relation
    public virtual List<Emprestimo> Emprestimos { get; set; } = new();
}