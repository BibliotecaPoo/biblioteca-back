﻿using AutoMapper;
using Biblioteca.Application.Configuration;
using Biblioteca.Application.Contracts.Services;
using Biblioteca.Application.DTOs.Livro;
using Biblioteca.Application.Notifications;
using Biblioteca.Domain.Contracts.Repositories;
using Biblioteca.Domain.Entities;
using Biblioteca.Domain.Enums;
using Biblioteca.Domain.Validators.Livro;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Biblioteca.Application.Services;

public class LivroService : BaseService, ILivroService
{
    private readonly ILivroRepository _livroRepository;
    private readonly string _imageFolderPath;

    public LivroService(INotificator notificator, IMapper mapper, ILivroRepository livroRepository,
        IOptions<StorageSettings> storageSettings) : base(notificator, mapper)
    {
        _livroRepository = livroRepository;
        _imageFolderPath = storageSettings.Value.ImageFolderPath;
    }

    public async Task<LivroDto?> Adicionar(AdicionarLivroDto dto)
    {
        if (!await ValidacoesParaAdicionarLivro(dto))
            return null;

        var adicionarLivro = Mapper.Map<Livro>(dto);
        adicionarLivro.QuantidadeExemplaresDisponiveisParaEmprestimo = adicionarLivro.QuantidadeExemplaresDisponiveisEmEstoque;
        adicionarLivro.StatusLivro = EStatusLivro.Disponivel;

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

            if (!string.IsNullOrEmpty(livro.NomeArquivoCapa))
            {
                var caminhoImagemAnterior = Path.Combine(_imageFolderPath, livro.NomeArquivoCapa);
                if (File.Exists(caminhoImagemAnterior))
                    File.Delete(caminhoImagemAnterior);
            }

            var nomeArquivo = DateTime.Now.Ticks + "_" + Path.GetFileName(file.FileName);
            var caminhoCompleto = Path.Combine(_imageFolderPath, nomeArquivo);

            await using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            livro.NomeArquivoCapa = nomeArquivo;
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

    public async Task<List<LivroDto>> ObterPorTitulo(string titulo)
    {
        var obterLivros = await _livroRepository.ObterPorTitulo(titulo);
        return Mapper.Map<List<LivroDto>>(obterLivros);
    }

    public async Task<List<LivroDto>> ObterPorAutor(string autor)
    {
        var obterLivros = await _livroRepository.ObterPorAutor(autor);
        return Mapper.Map<List<LivroDto>>(obterLivros);
    }

    public async Task<List<LivroDto>> ObterPorEditora(string editora)
    {
        var obterLivros = await _livroRepository.ObterPorEditora(editora);
        return Mapper.Map<List<LivroDto>>(obterLivros);
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

        var livroIgual = await _livroRepository.FirstOrDefault(l =>
            l.Titulo == dto.Titulo && 
            l.Autor == dto.Autor && 
            l.Edicao == dto.Edicao && 
            l.Editora == dto.Editora);
        if (livroIgual != null)
        {
            Notificator.Handle("Já existe um livro cadastrado com o título, autor, edição e editora informados.");
            return false;
        }

        return true;
    }

    private async Task<bool> ValidacoesParaAtualizarLivro(int id, AtualizarLivroDto dto)
    {
        var livroExistente = await _livroRepository.ObterPorId(id);
        if (livroExistente == null)
        {
            Notificator.HandleNotFoundResource();
            return false;
        }

        if (livroExistente.QuantidadeExemplaresDisponiveisParaEmprestimo < livroExistente.QuantidadeExemplaresDisponiveisEmEstoque)
        {
            Notificator.Handle("Não é possível atualizar um livro que tenha algum exemplar emprestado ou renovado.");
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

        if (dto.AnoPublicacao.HasValue)
        {
            if (dto.AnoPublicacao <= 0)
            {
                Notificator.Handle("O ano de publicação deve ser maior que 0.");
                return false;
            }

            if (dto.AnoPublicacao > DateTime.Now.Year)
            {
                Notificator.Handle("O ano de publicação não pode ser no futuro.");
                return false;
            }
        }

        if (dto.QuantidadeExemplaresDisponiveisEmEstoque.HasValue && dto.QuantidadeExemplaresDisponiveisEmEstoque <= 0)
        {
            Notificator.Handle("A quantidade de exemplares deve ser maior que 0.");
            return false;
        }

        if (!string.IsNullOrEmpty(dto.Titulo) && !string.IsNullOrEmpty(dto.Autor) &&
            !string.IsNullOrEmpty(dto.Edicao) && !string.IsNullOrEmpty(dto.Editora))
        {
            var livroIgual = await _livroRepository.FirstOrDefault(l =>
                l.Titulo == dto.Titulo &&
                l.Autor == dto.Autor &&
                l.Edicao == dto.Edicao &&
                l.Editora == dto.Editora);
            if (livroIgual != null)
            {
                Notificator.Handle("Já existe um livro cadastrado com o título, autor, edição e editora informados.");
                return false;
            }
        }

        if (string.IsNullOrEmpty(dto.Titulo) && string.IsNullOrEmpty(dto.Autor) &&
            string.IsNullOrEmpty(dto.Edicao) && string.IsNullOrEmpty(dto.Editora) &&
            !dto.AnoPublicacao.HasValue && !dto.QuantidadeExemplaresDisponiveisEmEstoque.HasValue)
        {
            Notificator.Handle("Nenhum campo fornecido para atualização.");
            return false;
        }

        return true;
    }

    private void MappingParaAtualizarLivro(Livro livro, AtualizarLivroDto dto)
    {
        if (!string.IsNullOrEmpty(dto.Titulo))
            livro.Titulo = dto.Titulo;

        if (!string.IsNullOrEmpty(dto.Autor))
            livro.Autor = dto.Autor;

        if (!string.IsNullOrEmpty(dto.Edicao))
            livro.Edicao = dto.Edicao;

        if (!string.IsNullOrEmpty(dto.Editora))
            livro.Editora = dto.Editora;

        if (dto.AnoPublicacao.HasValue)
            livro.AnoPublicacao = (int)dto.AnoPublicacao;

        if (dto.QuantidadeExemplaresDisponiveisEmEstoque.HasValue)
        {
            livro.QuantidadeExemplaresDisponiveisEmEstoque = (int)dto.QuantidadeExemplaresDisponiveisEmEstoque;
            livro.QuantidadeExemplaresDisponiveisParaEmprestimo = livro.QuantidadeExemplaresDisponiveisEmEstoque;
        }
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