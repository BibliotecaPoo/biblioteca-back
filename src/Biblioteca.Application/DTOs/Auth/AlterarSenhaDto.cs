namespace Biblioteca.Application.DTOs.Auth;

public class AlterarSenhaDto
{
    public string CodigoParaAlterarSenha { get; set; } = null!;
    public string NovaSenha { get; set; } = null!;
    public string ConfirmarNovaSenha { get; set; } = null!;
}