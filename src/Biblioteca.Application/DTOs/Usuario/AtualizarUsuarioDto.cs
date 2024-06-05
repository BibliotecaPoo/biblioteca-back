namespace Biblioteca.Application.DTOs.Usuario;

public class AtualizarUsuarioDto
{
    public int Id { get; set; }
    public string? Nome { get; set; }
    public string? Matricula { get; set; }
    public string? Email { get; set; }
    public string? Senha { get; set; }
}