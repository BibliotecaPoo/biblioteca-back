using FluentValidation;

namespace Biblioteca.Domain.Validators.Livro;

public class ValidadorParaAdicionarLivro : AbstractValidator<Entities.Livro>
{
    public ValidadorParaAdicionarLivro()
    {
        RuleFor(l => l.Titulo)
            .NotNull()
            .WithMessage("O título não pode ser nulo.")
            .Length(3, 100)
            .WithMessage("O título deve conter entre {MinLength} e {MaxLength} caracteres.");

        RuleFor(l => l.Descricao)
            .NotNull()
            .WithMessage("A descrição não pode ser nula.")
            .Length(20, 200)
            .WithMessage("A descrição deve conter entre {MinLength} e {MaxLength} caracteres.");

        RuleFor(l => l.Autor)
            .NotNull()
            .WithMessage("O autor não pode ser nulo.")
            .Length(3, 50)
            .WithMessage("O autor deve conter entre {MinLength} e {MaxLength} caracteres.");

        RuleFor(l => l.Editora)
            .NotNull()
            .WithMessage("A editora não pode ser nula.")
            .Length(3, 50)
            .WithMessage("A editora deve conter entre {MinLength} e {MaxLength} caracteres.");

        RuleFor(l => l.AnoPublicacao)
            .NotNull()
            .WithMessage("O ano de publicação não pode ser nulo.")
            .GreaterThanOrEqualTo(1000)
            .WithMessage("O ano de publicação deve ser a partir do ano 1000.");
    }
}