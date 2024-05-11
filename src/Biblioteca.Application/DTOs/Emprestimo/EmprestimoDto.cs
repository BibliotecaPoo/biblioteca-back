namespace Biblioteca.Application.DTOs.Emprestimo;

public class EmprestimoDto
{
    public int Id { get; set; }
    public DateTime DataEmprestimo { get; set; }
    public DateTime DataDevolucaoPrevista { get; set; }
    public DateTime? DataDevolucaoRealizada { get; set; }
    public string StatusEmprestimo { get; set; } = null!;
    public int UsuarioId { get; set; }
    public int LivroId { get; set; }
}