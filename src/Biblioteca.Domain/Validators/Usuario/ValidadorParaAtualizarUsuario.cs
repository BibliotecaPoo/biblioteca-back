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
            .Matches("^[0-9]{6}$")
            .WithMessage("A matrícula deve conter exatamente 6 dígitos numéricos.")
            .When(u => !string.IsNullOrEmpty(u.Matricula));

        RuleFor(u => u.Curso)
            .Length(3, 100)
            .WithMessage("O curso deve conter entre {MinLength} e {MaxLength} caracteres.")
            .When(u => !string.IsNullOrEmpty(u.Curso));

        RuleFor(u => u.Email)
            .EmailAddress()
            .WithMessage("O email fornecido não é válido.")
            .MaximumLength(100)
            .WithMessage("O email deve conter no máximo {MaxLength} caracteres.")
            .When(u => !string.IsNullOrEmpty(u.Email));

        RuleFor(u => u.Senha)
            .Matches("^[0-9]{6}$")
            .WithMessage("A senha deve conter exatamente 6 dígitos numéricos.")
            .When(u => !string.IsNullOrEmpty(u.Senha));
    }
}