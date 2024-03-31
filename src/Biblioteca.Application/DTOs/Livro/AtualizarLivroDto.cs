﻿namespace Biblioteca.Application.DTOs.Livro;

public class AtualizarLivroDto
{
    public int Id { get; set; }
    public string? Titulo { get; set; }
    public string? Descricao { get; set; }
    public string? Autor { get; set; }
    public string? Editora { get; set; }
    public int? AnoPublicacao { get; set; }
}