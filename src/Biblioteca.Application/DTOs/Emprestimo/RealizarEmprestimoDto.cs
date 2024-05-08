namespace Biblioteca.Application.DTOs.Emprestimo;

public class RealizarEmprestimoDto
{
    public int LivroId { get; set; }
    public string UsuarioMatricula { get; set; } = null!;
    public string UsuarioSenha { get; set; } = null!;
}