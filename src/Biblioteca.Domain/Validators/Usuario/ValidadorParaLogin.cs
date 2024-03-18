using FluentValidation;

namespace Biblioteca.Domain.Validators.Usuario;

public class ValidadorParaLogin : AbstractValidator<Domain.Entities.Usuario>
{
    public ValidadorParaLogin()
    {
        RuleFor(u => u.Email)
            .NotEmpty()
            .WithMessage("O email não pode ser vazio.");

        RuleFor(u => u.Senha)
            .NotEmpty()
            .WithMessage("A senha não pode ser vazia.");
    }
}