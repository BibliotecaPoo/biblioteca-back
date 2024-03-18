using Biblioteca.Application.DTOs.Auth;
using Biblioteca.Application.DTOs.Usuario;

namespace Biblioteca.Application.Contracts.Services;

public interface IAuthService
{
    Task<UsuarioDto?> Registrar(AdicionarUsuarioDto dto);
    Task<TokenDto?> Login(LoginDto dto);
}