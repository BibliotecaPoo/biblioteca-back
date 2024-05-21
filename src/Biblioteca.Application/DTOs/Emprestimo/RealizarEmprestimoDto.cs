namespace Biblioteca.Application.DTOs.Emprestimo;

public class RealizarEmprestimoDto
{
    public string UsuarioMatricula { get; set; } = null!;
    public string UsuarioSenha { get; set; } = null!;
    public int LivroId { get; set; }
}