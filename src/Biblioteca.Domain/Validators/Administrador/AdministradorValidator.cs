using FluentValidation;

namespace Biblioteca.Domain.Validators.Administrador;

public class AdministradorValidator : AbstractValidator<Domain.Entities.Administrador>
{
    public AdministradorValidator()
    {
        RuleFor(a => a.Email)
            .NotEmpty()
            .WithMessage("O email não pode ser vazio.");

        RuleFor(a => a.Senha)
            .NotEmpty()
            .WithMessage("A senha não pode ser vazia.");
    }
}

public class SenhaAdministradorValidator : AbstractValidator<string>
{
    public SenhaAdministradorValidator()
    {
        RuleFor(s => s)
            .NotNull()
            .WithMessage("A senha não pode ser nula.")
            .Matches("^[0-9]{6}$")
            .WithMessage("A senha deve conter exatamente 6 dígitos numéricos.");
    }
}