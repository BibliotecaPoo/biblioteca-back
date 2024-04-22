using AutoMapper;
using Biblioteca.Application.Contracts.Services;
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

        _usuarioRepository.Adicionar(adicionarUsuario);
        return await CommitChanges() ? Mapper.Map<UsuarioDto>(adicionarUsuario) : null;
    }

    public async Task<UsuarioDto?> Atualizar(int id, AtualizarUsuarioDto dto)
    {
        if (!await ValidacoesParaAtualizarUsuario(id, dto))
            return null;

        var atualizarUsuario = await _usuarioRepository.ObterPorId(id);
        MappingParaAtualizarUsuario(atualizarUsuario!, dto);

        _usuarioRepository.Atualizar(atualizarUsuario!);
        return await CommitChanges() ? Mapper.Map<UsuarioDto>(atualizarUsuario) : null;
    }

    public async Task<UsuarioDto?> ObterPorId(int id)
    {
        var obterUsuario = await _usuarioRepository.ObterPorId(id);
        if (obterUsuario != null)
            return Mapper.Map<UsuarioDto>(obterUsuario);

        Notificator.HandleNotFoundResource();
        return null;
    }

    public async Task<List<UsuarioDto>> ObterPorEmail(string email)
    {
        var obterUsuarios = await _usuarioRepository.ObterPorEmail(email);
        return Mapper.Map<List<UsuarioDto>>(obterUsuarios);
    }

    public async Task<List<UsuarioDto>> ObterPorMatricula(string matricula)
    {
        var obterUsuarios = await _usuarioRepository.ObterPorMatricula(matricula);
        return Mapper.Map<List<UsuarioDto>>(obterUsuarios);
    }

    public async Task<List<UsuarioDto>> ObterTodos()
    {
        var obterUsuarios = await _usuarioRepository.ObterTodos();
        return Mapper.Map<List<UsuarioDto>>(obterUsuarios);
    }

    public async Task Reativar(int id)
    {
        var reativarUsuario = await _usuarioRepository.ObterPorId(id);
        if (reativarUsuario == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }

        reativarUsuario.Ativo = true;

        _usuarioRepository.Atualizar(reativarUsuario);
        await CommitChanges();
    }

    public async Task Desativar(int id)
    {
        var desativarUsuario = await _usuarioRepository.ObterPorId(id);
        if (desativarUsuario == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }

        desativarUsuario.Ativo = false;

        _usuarioRepository.Atualizar(desativarUsuario);
        await CommitChanges();
    }

    private async Task<bool> ValidacoesParaAdicionarUsuario(AdicionarUsuarioDto dto)
    {
        var usuario = Mapper.Map<Usuario>(dto);
        var validador = new ValidadorParaAdicionarUsuario();

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
            Notificator.Handle("Os ids não conferem.");
            return false;
        }

        var usuarioExistente = await _usuarioRepository.ObterPorId(id);
        if (usuarioExistente == null)
        {
            Notificator.HandleNotFoundResource();
            return false;
        }

        var usuario = Mapper.Map<Usuario>(dto);
        var validador = new ValidadorParaAtualizarUsuario();

        var resultadoDaValidacao = await validador.ValidateAsync(usuario);
        if (!resultadoDaValidacao.IsValid)
        {
            Notificator.Handle(resultadoDaValidacao.Errors);
            return false;
        }

        if (!string.IsNullOrEmpty(dto.Matricula))
        {
            var usuarioComMatriculaExistente = await _usuarioRepository.FirstOrDefault(u => u.Matricula == dto.Matricula);
            if (usuarioComMatriculaExistente != null)
            {
                Notificator.Handle("Já existe um usuário cadastrado com a matrícula informada.");
                return false;
            }
        }

        if (!string.IsNullOrEmpty(dto.Email))
        {
            var usuarioComEmailExistente = await _usuarioRepository.FirstOrDefault(u => u.Email == dto.Email);
            if (usuarioComEmailExistente != null)
            {
                Notificator.Handle("Já existe um usuário cadastrado com o email informado.");
                return false;
            }
        }

        if (string.IsNullOrEmpty(dto.Nome) && string.IsNullOrEmpty(dto.Matricula) && string.IsNullOrEmpty(dto.Email) &&
            string.IsNullOrEmpty(dto.Senha))
        {
            Notificator.Handle("Nenhum campo fornecido para atualização.");
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