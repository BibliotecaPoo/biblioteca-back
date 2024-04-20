namespace Biblioteca.Application.DTOs.Usuario;

public class UsuarioDto
{
    public int Id { get; set; }
    public string Matricula { get; set; } = null!;
    public string Nome { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool Ativo { get; set; }
}