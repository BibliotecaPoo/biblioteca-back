namespace Biblioteca.Application.DTOs.Emprestimo;

public class RealizarEmprestimoDto
{
    public int IdLivro { get; set; }
    public string MatriculaUsuario { get; set; } = null!;
    public string SenhaUsuario { get; set; } = null!;
}