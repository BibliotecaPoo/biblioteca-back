namespace Biblioteca.Application.DTOs.Emprestimo;

public class RealizarRenovacaoDto
{
    public int Id { get; set; }
    public string UsuarioMatricula { get; set; } = null!;
    public string UsuarioSenha { get; set; } = null!;
    public int LivroCodigo { get; set; }
}