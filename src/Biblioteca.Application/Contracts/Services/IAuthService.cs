using Biblioteca.Application.DTOs.Auth;

namespace Biblioteca.Application.Contracts.Services;

public interface IAuthService
{
    Task<TokenDto?> Login(LoginDto dto);
}