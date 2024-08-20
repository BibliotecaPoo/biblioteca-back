using Biblioteca.Application.DTOs.Livro;
using Biblioteca.Application.DTOs.Paginacao;
using Microsoft.AspNetCore.Http;

namespace Biblioteca.Application.Contracts.Services;

public interface ILivroService
{
    Task<LivroDto?> Adicionar(AdicionarLivroDto dto);
    Task<LivroDto?> Atualizar(int id, AtualizarLivroDto dto);
    Task<LivroDto?> UploadCapa(int id, ICollection<IFormFile>? files);
    Task<PaginacaoDto<LivroDto>> Pesquisar(PesquisarLivroDto dto);
    Task<List<LivroDto>> ObterTodos();
    Task Ativar(int id);
    Task Desativar(int id);
}