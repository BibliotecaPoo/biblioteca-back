using FluentValidation;

namespace Biblioteca.Domain.Validators.Livro;

public class ValidadorParaAtualizarLivro : AbstractValidator<Entities.Livro>
{
    public ValidadorParaAtualizarLivro()
    {
        RuleFor(l => l.Titulo)
            .Length(3, 100)
            .WithMessage("O título deve conter entre {MinLength} e {MaxLength} caracteres.")
            .When(l => !string.IsNullOrEmpty(l.Titulo));

        RuleFor(l => l.Descricao)
            .Length(20, 200)
            .WithMessage("A descrição deve conter entre {MinLength} e {MaxLength} caracteres.")
            .When(l => !string.IsNullOrEmpty(l.Descricao));

        RuleFor(l => l.Autor)
            .Length(3, 50)
            .WithMessage("O autor deve conter entre {MinLength} e {MaxLength} caracteres.")
            .When(l => !string.IsNullOrEmpty(l.Autor));

        RuleFor(l => l.Editora)
            .Length(3, 50)
            .WithMessage("A editora deve conter entre {MinLength} e {MaxLength} caracteres.")
            .When(l => !string.IsNullOrEmpty(l.Editora));
    }
}