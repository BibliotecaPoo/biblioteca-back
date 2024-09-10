using Biblioteca.Application.DTOs.Auth;

namespace Biblioteca.Application.Contracts.Services;

public interface IAuthService
{
    Task<TokenDto?> Login(LoginDto dto);
    Task<bool> EsqueceuSenha(string email);
    Task<bool> AlterarSenha(AlterarSenhaDto dto);
}