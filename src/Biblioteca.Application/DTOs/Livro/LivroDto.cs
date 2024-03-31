namespace Biblioteca.Application.DTOs.Livro;

public class LivroDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public string Autor { get; set; } = null!;
    public string Editora { get; set; } = null!;
    public int AnoPublicacao { get; set; }
    public string? Capa { get; set; }
}