using AutoMapper;
using Biblioteca.Application.Contracts.Services;
using Biblioteca.Application.DTOs.Emprestimo;
using Biblioteca.Application.Notifications;
using Biblioteca.Domain.Contracts.Repositories;
using Biblioteca.Domain.Entities;
using Biblioteca.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Biblioteca.Application.Services;

public class EmprestimoService : BaseService, IEmprestimoService
{
    private readonly IEmprestimoRepository _emprestimoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ILivroRepository _livroRepository;
    private readonly IPasswordHasher<Usuario> _passwordHasher;

    public EmprestimoService(INotificator notificator, IMapper mapper, IEmprestimoRepository emprestimoRepository,
        IUsuarioRepository usuarioRepository, ILivroRepository livroRepository,
        IPasswordHasher<Usuario> passwordHasher) : base(notificator, mapper)
    {
        _emprestimoRepository = emprestimoRepository;
        _usuarioRepository = usuarioRepository;
        _livroRepository = livroRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<EmprestimoDto?> RealizarEmprestimo(RealizarEmprestimoDto dto)
    {
        var usuario = await _usuarioRepository.FirstOrDefault(u => u.Matricula == dto.UsuarioMatricula);
        if (usuario == null)
        {
            Notificator.Handle("Usuário não encontrado com a matrícula informada.");
            return null;
        }

        if (usuario.Bloqueado)
        {
            Notificator.Handle("O usuário está temporariamente impedido de realizar empréstimos ou renovações.");
            return null;
        }

        var emprestimoAtrasado = await _emprestimoRepository.FirstOrDefault(e =>
            e.UsuarioId == usuario.Id &&
            DateTime.Today > e.DataDevolucaoPrevista &&
            (e.StatusEmprestimo == EStatusEmprestimo.Emprestado || e.StatusEmprestimo == EStatusEmprestimo.Renovado));
        if (emprestimoAtrasado != null)
        {
            usuario.Bloqueado = true;
            _usuarioRepository.Atualizar(usuario);

            Notificator.Handle("O usuário possui livro(s) não devolvido(s) e atrasado(s).");
            Notificator.Handle("O usuário será impedido de fazer empréstimos ou renovações até devolver o(s) livro(s).");
            return null;
        }

        var emprestimoIgual = await _emprestimoRepository.FirstOrDefault(e =>
            e.LivroId == dto.LivroId &&
            e.UsuarioId == usuario.Id &&
            (e.StatusEmprestimo == EStatusEmprestimo.Emprestado || e.StatusEmprestimo == EStatusEmprestimo.Renovado));
        if (emprestimoIgual != null)
        {
            Notificator.Handle("No momento o usuário já possui um exemplar emprestado ou renovado desse mesmo livro.");
            return null;
        }

        if (usuario.QuantidadeEmprestimosRealizados == usuario.QuantidadeEmprestimosPermitida)
        {
            Notificator.Handle("O usuário já atingiu o limite de empréstimos.");
            return null;
        }

        var livro = await _livroRepository.ObterPorId(dto.LivroId);
        if (livro == null)
        {
            Notificator.Handle("Livro não encontrado.");
            return null;
        }

        if (livro.StatusLivro == EStatusLivro.Indisponivel)
        {
            Notificator.Handle("Não existe exemplar disponível no momento para esse livro.");
            return null;
        }

        var resultadoVerificacaoSenha = _passwordHasher.VerifyHashedPassword(usuario, usuario.Senha, dto.UsuarioSenha);
        if (resultadoVerificacaoSenha == PasswordVerificationResult.Failed)
        {
            Notificator.Handle("Senha incorreta.");
            return null;
        }

        livro.QuantidadeExemplaresDisponiveisParaEmprestimo -= 1;
        if (livro.QuantidadeExemplaresDisponiveisParaEmprestimo == 0)
        {
            livro.StatusLivro = EStatusLivro.Indisponivel;
        }
        _livroRepository.Atualizar(livro);

        usuario.QuantidadeEmprestimosRealizados += 1;
        _usuarioRepository.Atualizar(usuario);

        var emprestimo = Mapper.Map<Emprestimo>(dto);
        emprestimo.DataEmprestimo = DateTime.Today;
        emprestimo.DataDevolucaoPrevista = DateTime.Today.AddDays(10);
        emprestimo.StatusEmprestimo = EStatusEmprestimo.Emprestado;
        emprestimo.QuantidadeRenovacoesPermitida = 5;
        emprestimo.UsuarioId = usuario.Id;

        _emprestimoRepository.Adicionar(emprestimo);
        return await CommitChanges() ? Mapper.Map<EmprestimoDto>(emprestimo) : null;
    }

    public async Task<EmprestimoDto?> RealizarRenovacao(int id, RealizarRenovacaoOuEntregaDto dto)
    {
        var emprestimo = await _emprestimoRepository.ObterPorId(id);
        if (emprestimo == null)
        {
            Notificator.HandleNotFoundResource();
            return null;
        }

        var usuario = await _usuarioRepository.ObterPorId(emprestimo.UsuarioId);
        if (usuario!.Bloqueado)
        {
            Notificator.Handle("O usuário está temporariamente impedido de realizar empréstimos ou renovações.");
            return null;
        }

        var emprestimoAtrasado = await _emprestimoRepository.FirstOrDefault(e =>
            e.UsuarioId == usuario.Id &&
            DateTime.Today > e.DataDevolucaoPrevista &&
            (e.StatusEmprestimo == EStatusEmprestimo.Emprestado || e.StatusEmprestimo == EStatusEmprestimo.Renovado));
        if (emprestimoAtrasado != null)
        {
            usuario.Bloqueado = true;
            _usuarioRepository.Atualizar(usuario);

            Notificator.Handle("O usuário possui livro(s) não devolvido(s) e atrasado(s).");
            Notificator.Handle("O usuário será impedido de fazer empréstimos ou renovações até devolver o(s) livro(s).");
            return null;
        }

        if (emprestimo.QuantidadeRenovacoesRealizadas == emprestimo.QuantidadeRenovacoesPermitida)
        {
            Notificator.Handle("O usuário já atingiu o limite de renovações para esse livro.");
            return null;
        }

        if (emprestimo.StatusEmprestimo == EStatusEmprestimo.Entregue)
        {
            Notificator.Handle("O empréstimo já está como entregue. Não é possível renovar o livro.");
            return null;
        }

        var resultadoVerificacaoSenha = _passwordHasher.VerifyHashedPassword(usuario, usuario.Senha, dto.UsuarioSenha);
        if (resultadoVerificacaoSenha == PasswordVerificationResult.Failed)
        {
            Notificator.Handle("Senha incorreta.");
            return null;
        }

        emprestimo.DataEmprestimo = DateTime.Today;
        emprestimo.DataDevolucaoPrevista = DateTime.Today.AddDays(10);
        emprestimo.StatusEmprestimo = EStatusEmprestimo.Renovado;
        emprestimo.QuantidadeRenovacoesRealizadas += 1;

        _emprestimoRepository.Atualizar(emprestimo);
        return await CommitChanges() ? Mapper.Map<EmprestimoDto>(emprestimo) : null;
    }

    public async Task<EmprestimoDto?> RealizarEntrega(int id, RealizarRenovacaoOuEntregaDto dto)
    {
        var emprestimo = await _emprestimoRepository.ObterPorId(id);
        if (emprestimo == null)
        {
            Notificator.HandleNotFoundResource();
            return null;
        }

        if (emprestimo.StatusEmprestimo is EStatusEmprestimo.Entregue or EStatusEmprestimo.EntregueComAtraso)
        {
            Notificator.Handle("O empréstimo já está como entregue.");
            return null;
        }

        var usuario = await _usuarioRepository.ObterPorId(emprestimo.UsuarioId);

        var resultadoVerificacaoSenha = _passwordHasher.VerifyHashedPassword(usuario!, usuario!.Senha, dto.UsuarioSenha);
        if (resultadoVerificacaoSenha == PasswordVerificationResult.Failed)
        {
            Notificator.Handle("Senha incorreta.");
            return null;
        }

        var livro = await _livroRepository.ObterPorId(emprestimo.LivroId);
        if (livro!.StatusLivro == EStatusLivro.Indisponivel)
        {
            livro.StatusLivro = EStatusLivro.Disponivel;
        }

        livro.QuantidadeExemplaresDisponiveisParaEmprestimo += 1;
        _livroRepository.Atualizar(livro);

        usuario.QuantidadeEmprestimosRealizados -= 1;
        _usuarioRepository.Atualizar(usuario);

        emprestimo.DataDevolucaoRealizada = DateTime.Today;
        emprestimo.StatusEmprestimo = emprestimo.DataDevolucaoRealizada > emprestimo.DataDevolucaoPrevista
            ? EStatusEmprestimo.EntregueComAtraso
            : EStatusEmprestimo.Entregue;
        
        _emprestimoRepository.Atualizar(emprestimo);
        
        if (usuario.Bloqueado)
        {
            var emprestimoAtrasado = await _emprestimoRepository.FirstOrDefault(e =>
                e.UsuarioId == usuario.Id &&
                DateTime.Today > e.DataDevolucaoPrevista &&
                (e.StatusEmprestimo == EStatusEmprestimo.Emprestado || e.StatusEmprestimo == EStatusEmprestimo.Renovado));
            
            if (emprestimoAtrasado == null)
            {
                usuario.Bloqueado = false;
                _usuarioRepository.Atualizar(usuario);
            }
        }
        
        return await CommitChanges() ? Mapper.Map<EmprestimoDto>(emprestimo) : null;
    }

    public async Task<EmprestimoDto?> ObterPorId(int id)
    {
        var obterEmprestimo = await _emprestimoRepository.ObterPorId(id);
        if (obterEmprestimo != null)
            return Mapper.Map<EmprestimoDto>(obterEmprestimo);

        Notificator.HandleNotFoundResource();
        return null;
    }

    public async Task<List<EmprestimoDto>> ObterTodos()
    {
        var obterEmprestimos = await _emprestimoRepository.ObterTodos();
        return Mapper.Map<List<EmprestimoDto>>(obterEmprestimos);
    }

    public async Task<List<EmprestimoDto>?> ObterHistoricoDeEmprestimoDeUmUsuario(int usuarioId)
    {
        var usuario = await _usuarioRepository.ObterPorId(usuarioId);
        if (usuario == null)
        {
            Notificator.Handle("Usuário não encontrado.");
            return null;
        }

        var obterEmprestimos = await _emprestimoRepository.ObterHistoricoDeEmprestimoDeUmUsuario(usuarioId);
        return Mapper.Map<List<EmprestimoDto>>(obterEmprestimos);
    }

    public async Task<List<EmprestimoDto>?> ObterHistoricoDeEmprestimoDeUmUsuario(string usuarioMatricula)
    {
        var usuario = await _usuarioRepository.FirstOrDefault(u => u.Matricula == usuarioMatricula);
        if (usuario == null)
        {
            Notificator.Handle("Usuário não encontrado.");
            return null;
        }

        var obterEmprestimos = await _emprestimoRepository.ObterHistoricoDeEmprestimoDeUmUsuario(usuarioMatricula);
        return Mapper.Map<List<EmprestimoDto>>(obterEmprestimos);
    }

    public async Task<List<EmprestimoDto>?> ObterHistoricoDeEmprestimoDeUmLivro(int livroId)
    {
        var livro = await _livroRepository.ObterPorId(livroId);
        if (livro == null)
        {
            Notificator.Handle("Livro não encontrado.");
            return null;
        }

        var obterEmprestimos = await _emprestimoRepository.ObterHistoricoDeEmprestimoDeUmLivro(livroId);
        return Mapper.Map<List<EmprestimoDto>>(obterEmprestimos);
    }

    private async Task<bool> CommitChanges()
    {
        if (await _emprestimoRepository.UnitOfWork.Commit())
            return true;

        Notificator.Handle("Ocorreu um erro ao salvar as alterações.");
        return false;
    }
}