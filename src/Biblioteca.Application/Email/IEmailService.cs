using Biblioteca.Domain.Entities;

namespace Biblioteca.Application.Email;

public interface IEmailService
{
    Task EnviarEmailParaRecuperarSenhaDoAdministrador(Administrador administrador);
}