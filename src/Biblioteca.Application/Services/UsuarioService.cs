﻿using AutoMapper;
using Biblioteca.Application.Contracts.Services;
using Biblioteca.Application.DTOs.Paginacao;
using Biblioteca.Application.DTOs.Usuario;
using Biblioteca.Application.Notifications;
using Biblioteca.Domain.Contracts.Repositories;
using Biblioteca.Domain.Entities;
using Biblioteca.Domain.Validators.Usuario;
using Microsoft.AspNetCore.Identity;

namespace Biblioteca.Application.Services;

public class UsuarioService : BaseService, IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher<Usuario> _passwordHasher;

    public UsuarioService(INotificator notificator, IMapper mapper, IUsuarioRepository usuarioRepository,
        IPasswordHasher<Usuario> passwordHasher) : base(notificator, mapper)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<UsuarioDto?> Adicionar(AdicionarUsuarioDto dto)
    {
        if (!await ValidacoesParaAdicionarUsuario(dto))
            return null;

        var adicionarUsuario = Mapper.Map<Usuario>(dto);
        adicionarUsuario.Senha = _passwordHasher.HashPassword(adicionarUsuario, dto.Senha);
        adicionarUsuario.QuantidadeEmprestimosPermitida = 5;

        _usuarioRepository.Adicionar(adicionarUsuario);
        return await CommitChanges() ? Mapper.Map<UsuarioDto>(adicionarUsuario) : null;
    }

    public async Task<UsuarioDto?> Atualizar(int id, AtualizarUsuarioDto dto)
    {
        if (!await ValidacoesParaAtualizarUsuario(id, dto))
            return null;

        var atualizarUsuario = await _usuarioRepository.FirstOrDefault(u => u.Id == id);
        MappingParaAtualizarUsuario(atualizarUsuario!, dto);

        _usuarioRepository.Atualizar(atualizarUsuario!);
        return await CommitChanges() ? Mapper.Map<UsuarioDto>(atualizarUsuario) : null;
    }

    public async Task Deletar(int id)
    {
        var obterUsuario = await _usuarioRepository.FirstOrDefault(u => u.Id == id);
        if (obterUsuario == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }

        if (obterUsuario.QuantidadeEmprestimosRealizados > 0)
        {
            Notificator.Handle("O usuário possui livros emprestados, não é possível deletá-lo.");
            return;
        }

        _usuarioRepository.Deletar(obterUsuario);
        await CommitChanges();
    }

    public async Task<PaginacaoDto<UsuarioDto>> Pesquisar(PesquisarUsuarioDto dto)
    {
        var resultadoPaginado = await _usuarioRepository.Pesquisar(dto.Id, dto.Nome, dto.Email, dto.Matricula,
            dto.Curso, dto.QuantidadeDeItensPorPagina, dto.PaginaAtual);

        return new PaginacaoDto<UsuarioDto>
        {
            TotalDeItens = resultadoPaginado.TotalDeItens,
            QuantidadeDeItensPorPagina = resultadoPaginado.QuantidadeDeItensPorPagina,
            QuantidadeDePaginas = resultadoPaginado.QuantidadeDePaginas,
            PaginaAtual = resultadoPaginado.PaginaAtual,
            Itens = Mapper.Map<List<UsuarioDto>>(resultadoPaginado.Itens)
        };
    }

    public async Task<List<UsuarioDto>> ObterTodos()
    {
        var obterUsuarios = await _usuarioRepository.ObterTodos();
        return Mapper.Map<List<UsuarioDto>>(obterUsuarios);
    }

    private async Task<bool> ValidacoesParaAdicionarUsuario(AdicionarUsuarioDto dto)
    {
        var usuario = Mapper.Map<Usuario>(dto);
        var validador = new UsuarioValidator();

        var resultadoDaValidacao = await validador.ValidateAsync(usuario);
        if (!resultadoDaValidacao.IsValid)
        {
            Notificator.Handle(resultadoDaValidacao.Errors);
            return false;
        }

        var usuarioComMatriculaExistente = await _usuarioRepository.FirstOrDefault(u => u.Matricula == dto.Matricula);
        if (usuarioComMatriculaExistente != null)
        {
            Notificator.Handle("Já existe um usuário cadastrado com a matrícula informada.");
            return false;
        }

        var usuarioComEmailExistente = await _usuarioRepository.FirstOrDefault(u => u.Email == dto.Email);
        if (usuarioComEmailExistente != null)
        {
            Notificator.Handle("Já existe um usuário cadastrado com o email informado.");
            return false;
        }

        return true;
    }

    private async Task<bool> ValidacoesParaAtualizarUsuario(int id, AtualizarUsuarioDto dto)
    {
        if (id != dto.Id)
        {
            Notificator.Handle("O id informado na url deve ser igual ao id informado no json.");
            return false;
        }

        var usuarioExistente = await _usuarioRepository.FirstOrDefault(u => u.Id == id);
        if (usuarioExistente == null)
        {
            Notificator.HandleNotFoundResource();
            return false;
        }

        var usuario = Mapper.Map<Usuario>(dto);
        var validador = new UsuarioValidator();

        var resultadoDaValidacao = await validador.ValidateAsync(usuario);
        if (!resultadoDaValidacao.IsValid)
        {
            Notificator.Handle(resultadoDaValidacao.Errors);
            return false;
        }

        return true;
    }

    private void MappingParaAtualizarUsuario(Usuario usuario, AtualizarUsuarioDto dto)
    {
        if (!string.IsNullOrEmpty(dto.Nome))
            usuario.Nome = dto.Nome;

        if (!string.IsNullOrEmpty(dto.Matricula))
            usuario.Matricula = dto.Matricula;

        if (!string.IsNullOrEmpty(dto.Curso))
            usuario.Curso = dto.Curso;

        if (!string.IsNullOrEmpty(dto.Email))
            usuario.Email = dto.Email;

        if (!string.IsNullOrEmpty(dto.Senha))
        {
            usuario.Senha = dto.Senha;
            usuario.Senha = _passwordHasher.HashPassword(usuario, dto.Senha);
        }
    }

    private async Task<bool> CommitChanges()
    {
        if (await _usuarioRepository.UnitOfWork.Commit())
            return true;

        Notificator.Handle("Ocorreu um erro ao salvar as alterações.");
        return false;
    }
}