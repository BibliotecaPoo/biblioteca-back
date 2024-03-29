using Biblioteca.Application.DTOs.Livro;
using Microsoft.AspNetCore.Http;

namespace Biblioteca.Application.Contracts.Services;

public interface ILivroService
{
    Task<LivroDto?> Adicionar(AdicionarLivroDto dto);
    Task<LivroDto?> AdicionarCapa(int id, ICollection<IFormFile>? files);
    Task<LivroDto?> Atualizar(int id, AtualizarLivroDto dto);
    Task<LivroDto?> ObterPorId(int id);
    Task<List<LivroDto>> ObterTodos();
}