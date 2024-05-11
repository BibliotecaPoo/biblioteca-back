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

        RuleFor(l => l.Autor)
            .Length(3, 50)
            .WithMessage("O autor deve conter entre {MinLength} e {MaxLength} caracteres.")
            .When(l => !string.IsNullOrEmpty(l.Autor));

        RuleFor(l => l.Edicao)
            .Matches(@"^[1-9][0-9]*[aª] edicao$")
            .WithMessage("A edição deve seguir um dos padrões: 1a edicao ou 1ª edicao, 2a edicao ou 2ª edicao...")
            .Length(3, 30)
            .WithMessage("A edição deve conter entre {MinLength} e {MaxLength} caracteres.")
            .When(l => !string.IsNullOrEmpty(l.Edicao));

        RuleFor(l => l.Editora)
            .Length(3, 50)
            .WithMessage("A editora deve conter entre {MinLength} e {MaxLength} caracteres.")
            .When(l => !string.IsNullOrEmpty(l.Editora));
    }
}