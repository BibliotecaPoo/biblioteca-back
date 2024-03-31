using AutoMapper;
using Biblioteca.Application.Contracts.Services;
using Biblioteca.Application.DTOs.Livro;
using Biblioteca.Application.Notifications;
using Biblioteca.Domain.Contracts.Repositories;
using Biblioteca.Domain.Entities;
using Biblioteca.Domain.Validators.Livro;
using Microsoft.AspNetCore.Http;

namespace Biblioteca.Application.Services;

public class LivroService : BaseService, ILivroService
{
    private readonly ILivroRepository _livroRepository;

    public LivroService(INotificator notificator, IMapper mapper, ILivroRepository livroRepository) : base(notificator,
        mapper)
    {
        _livroRepository = livroRepository;
    }

    public async Task<LivroDto?> Adicionar(AdicionarLivroDto dto)
    {
        if (!await ValidacoesParaAdicionarLivro(dto))
            return null;

        var adicionarLivro = Mapper.Map<Livro>(dto);
        _livroRepository.Adicionar(adicionarLivro);

        return await CommitChanges() ? Mapper.Map<LivroDto>(adicionarLivro) : null;
    }

    public async Task<LivroDto?> Atualizar(int id, AtualizarLivroDto dto)
    {
        if (!await ValidacoesParaAtualizarLivro(id, dto))
            return null;

        var atualizarLivro = await _livroRepository.ObterPorId(id);
        MappingParaAtualizarLivro(atualizarLivro!, dto);

        _livroRepository.Atualizar(atualizarLivro!);
        return await CommitChanges() ? Mapper.Map<LivroDto>(atualizarLivro) : null;
    }

    public async Task<LivroDto?> UploadCapa(int id, ICollection<IFormFile>? files)
    {
        var livro = await _livroRepository.ObterPorId(id);
        if (livro == null)
        {
            Notificator.HandleNotFoundResource();
            return null;
        }

        if (files == null || files.Count == 0)
        {
            Notificator.Handle("Nenhum arquivo enviado.");
            return null;
        }

        foreach (var file in files)
        {
            if (!EhImagem(file))
            {
                Notificator.Handle("Apenas arquivos de imagem são permitidos.");
                return null;
            }

            if (!string.IsNullOrEmpty(livro.Capa))
            {
                var caminhoImagemAnterior = Path.Combine("../../../imagens", livro.Capa);
                if (File.Exists(caminhoImagemAnterior))
                    File.Delete(caminhoImagemAnterior);
            }

            var nomeArquivo = DateTime.Now.Ticks + "_" + Path.GetFileName(file.FileName);
            var caminhoCompleto = Path.Combine("../../../imagens", nomeArquivo);

            using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            livro.Capa = nomeArquivo;

            _livroRepository.Atualizar(livro);
        }

        return await CommitChanges() ? Mapper.Map<LivroDto>(livro) : null;
    }

    public async Task<LivroDto?> ObterPorId(int id)
    {
        var obterLivro = await _livroRepository.ObterPorId(id);
        if (obterLivro != null)
            return Mapper.Map<LivroDto>(obterLivro);

        Notificator.HandleNotFoundResource();
        return null;
    }

    public async Task<List<LivroDto>> ObterTodos()
    {
        var obterLivros = await _livroRepository.ObterTodos();
        return Mapper.Map<List<LivroDto>>(obterLivros);
    }

    private async Task<bool> ValidacoesParaAdicionarLivro(AdicionarLivroDto dto)
    {
        var livro = Mapper.Map<Livro>(dto);
        var validador = new ValidadorParaAdicionarLivro();

        var resultadoDaValidacao = await validador.ValidateAsync(livro);
        if (!resultadoDaValidacao.IsValid)
        {
            Notificator.Handle(resultadoDaValidacao.Errors);
            return false;
        }

        var livroComTituloExistente = await _livroRepository.FirstOrDefault(l => l.Titulo == dto.Titulo);
        if (livroComTituloExistente != null)
        {
            Notificator.Handle("Já foi cadastrado um livro com o título informado.");
            return false;
        }

        return true;
    }

    private async Task<bool> ValidacoesParaAtualizarLivro(int id, AtualizarLivroDto dto)
    {
        if (id != dto.Id)
        {
            Notificator.Handle("Os ids não conferem.");
            return false;
        }

        var livroExistente = await _livroRepository.ObterPorId(id);
        if (livroExistente == null)
        {
            Notificator.HandleNotFoundResource();
            return false;
        }

        var livro = Mapper.Map<Livro>(dto);
        var validador = new ValidadorParaAtualizarLivro();

        var resultadoDaValidacao = await validador.ValidateAsync(livro);
        if (!resultadoDaValidacao.IsValid)
        {
            Notificator.Handle(resultadoDaValidacao.Errors);
            return false;
        }

        if (!string.IsNullOrEmpty(livro.Titulo))
        {
            var livroComTituloExistente = await _livroRepository.FirstOrDefault(l => l.Titulo == dto.Titulo);
            if (livroComTituloExistente != null)
            {
                Notificator.Handle("Já foi cadastrado um livro com o título informado.");
                return false;
            }
        }

        return true;
    }

    private void MappingParaAtualizarLivro(Livro livro, AtualizarLivroDto dto)
    {
        if (!string.IsNullOrEmpty(dto.Titulo))
            livro.Titulo = dto.Titulo;

        if (!string.IsNullOrEmpty(dto.Descricao))
            livro.Descricao = dto.Descricao;

        if (!string.IsNullOrEmpty(dto.Autor))
            livro.Autor = dto.Autor;

        if (!string.IsNullOrEmpty(dto.Editora))
            livro.Editora = dto.Editora;

        if (dto.AnoPublicacao.HasValue)
            livro.AnoPublicacao = (int)dto.AnoPublicacao;
    }

    private bool EhImagem(IFormFile file)
    {
        var extensoesPermitidas = new[] { ".jpg", ".jpeg", ".png" };
        var extensao = Path.GetExtension(file.FileName).ToLowerInvariant();

        return extensoesPermitidas.Contains(extensao);
    }

    private async Task<bool> CommitChanges()
    {
        if (await _livroRepository.UnitOfWork.Commit())
            return true;

        Notificator.Handle("Ocorreu um erro ao salvar as alterações.");
        return false;
    }
}