using FluentValidation;

namespace Biblioteca.Domain.Validators.Usuario;

public class ValidadorParaAtualizarUsuario : AbstractValidator<Domain.Entities.Usuario>
{
    public ValidadorParaAtualizarUsuario()
    {
        RuleFor(u => u.Nome)
            .Length(3, 50)
            .WithMessage("O nome deve conter entre {MinLength} e {MaxLength} caracteres.")
            .When(u => !string.IsNullOrEmpty(u.Nome));

        RuleFor(u => u.Matricula)
            .Length(10)
            .WithMessage("A matrícula deve conter exatamente 10 dígitos.")
            .When(u => !string.IsNullOrEmpty(u.Matricula));

        RuleFor(u => u.Email)
            .EmailAddress()
            .WithMessage("O email fornecido não é válido.")
            .MaximumLength(100)
            .WithMessage("O email deve conter no máximo {MaxLength} caracteres.")
            .When(u => !string.IsNullOrEmpty(u.Email));

        RuleFor(u => u.Senha)
            .MinimumLength(5)
            .WithMessage("A senha deve conter no mínimo {MinLength} caracteres.")
            .When(u => !string.IsNullOrEmpty(u.Senha));
    }
}