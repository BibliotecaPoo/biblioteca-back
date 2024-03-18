using Biblioteca.Core.Enum;
using Microsoft.AspNetCore.Http;

namespace Biblioteca.Core.Extensions;

public static class HttpContextAccessorExtension
{
    public static bool UsuarioAutenticado(this IHttpContextAccessor? contextAccessor)
        => contextAccessor?.HttpContext?.User.UsuarioAutenticado() ?? false;

    public static int? ObterUsuarioId(this IHttpContextAccessor? contextAccessor)
    {
        var id = contextAccessor?.HttpContext?.User.ObterUsuarioId() ?? string.Empty;
        return string.IsNullOrWhiteSpace(id) ? null : int.Parse(id);
    }

    public static string ObterNome(this IHttpContextAccessor? contextAccessor)
    {
        var nome = contextAccessor?.HttpContext?.User.ObterNomeUsuario() ?? string.Empty;
        return string.IsNullOrWhiteSpace(nome) ? string.Empty : nome;
    }

    public static string ObterEmail(this IHttpContextAccessor? contextAccessor)
    {
        var email = contextAccessor?.HttpContext?.User.ObterEmailUsuario() ?? string.Empty;
        return string.IsNullOrWhiteSpace(email) ? string.Empty : email;
    }

    public static ETipoUsuario? ObterTipoUsuario(this IHttpContextAccessor? contextAccessor)
    {
        var tipo = contextAccessor?.HttpContext?.User?.ObterTipoUsuario() ?? string.Empty;
        return string.IsNullOrWhiteSpace(tipo) ? null : System.Enum.Parse<ETipoUsuario>(tipo);
    }

    public static bool EhAdministrador(this IHttpContextAccessor? contextAccessor)
        => ObterTipoUsuario(contextAccessor) is ETipoUsuario.Administrador;
}