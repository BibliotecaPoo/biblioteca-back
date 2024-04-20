using Biblioteca.Application.DTOs.Livro;
using Microsoft.AspNetCore.Http;

namespace Biblioteca.Application.Contracts.Services;

public interface ILivroService
{
    Task<LivroDto?> Adicionar(AdicionarLivroDto dto);
    Task<LivroDto?> Atualizar(int id, AtualizarLivroDto dto);
    Task<LivroDto?> UploadCapa(int id, ICollection<IFormFile>? files);
    Task<LivroDto?> ObterPorId(int id);
    Task<List<LivroDto>> ObterPorTitulo(string titulo);
    Task<List<LivroDto>> ObterPorAutor(string autor);
    Task<List<LivroDto>> ObterPorEditora(string editora);
    Task<List<LivroDto>> ObterTodos();
}