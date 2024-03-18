namespace Biblioteca.Domain.Entities;

public class Usuario : Entity
{
    public string Nome { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Senha { get; set; } = null!;
    public bool Ativo { get; set; }
    public bool? SuperUsuario { get; set; }
}