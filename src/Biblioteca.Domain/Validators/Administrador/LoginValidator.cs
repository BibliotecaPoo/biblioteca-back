using FluentValidation;

namespace Biblioteca.Domain.Validators.Administrador;

public class LoginValidator : AbstractValidator<Domain.Entities.Administrador>
{
    public LoginValidator()
    {
        RuleFor(a => a.Email)
            .NotEmpty()
            .WithMessage("O email não pode ser vazio.");

        RuleFor(a => a.Senha)
            .NotEmpty()
            .WithMessage("A senha não pode ser vazia.");
    }
}