namespace Biblioteca.Domain.Entities;

public class Livro : Entity
{
    public string Titulo { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public string Autor { get; set; } = null!;
    public string Editora { get; set; } = null!;
    public int AnoPublicacao { get; set; }
    public string? CaminhoImagemCapa { get; set; }
}