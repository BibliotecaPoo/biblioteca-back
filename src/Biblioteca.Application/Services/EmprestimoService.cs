using AutoMapper;
using Biblioteca.Application.Contracts.Services;
using Biblioteca.Application.DTOs.Emprestimo;
using Biblioteca.Application.DTOs.Paginacao;
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

        if (usuario.Bloqueado == true)
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
            Notificator.Handle("O usuário será impedido de fazer empréstimos/renovações até devolver o(s) livro(s).");
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

        var livro = await _livroRepository.FirstOrDefault(l => l.Id == dto.LivroId);
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

    public async Task<EmprestimoDto?> RealizarRenovacao(int id, RealizarRenovacaoDto dto)
    {
        if (id != dto.Id)
        {
            Notificator.Handle("O id informado na url deve ser igual ao id informado no json.");
            return null;
        }

        var emprestimo = await _emprestimoRepository.FirstOrDefault(e => e.Id == id);
        if (emprestimo == null)
        {
            Notificator.HandleNotFoundResource();
            return null;
        }

        var usuario = await _usuarioRepository.FirstOrDefault(u => u.Id == emprestimo.UsuarioId);
        if (usuario!.Bloqueado == true)
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
            Notificator.Handle("O usuário será impedido de fazer empréstimos/renovações até devolver o(s) livro(s).");
            return null;
        }

        if (emprestimo.QuantidadeRenovacoesRealizadas == emprestimo.QuantidadeRenovacoesPermitida)
        {
            Notificator.Handle("O usuário já atingiu o limite de renovações para esse livro.");
            return null;
        }

        if (emprestimo.StatusEmprestimo is EStatusEmprestimo.Entregue or EStatusEmprestimo.EntregueComAtraso)
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

    public async Task<EmprestimoDto?> RealizarEntrega(int id)
    {
        var emprestimo = await _emprestimoRepository.FirstOrDefault(e => e.Id == id);
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

        var livro = await _livroRepository.FirstOrDefault(l => l.Id == emprestimo.LivroId);
        if (livro!.StatusLivro == EStatusLivro.Indisponivel)
        {
            livro.StatusLivro = EStatusLivro.Disponivel;
        }

        livro.QuantidadeExemplaresDisponiveisParaEmprestimo += 1;
        _livroRepository.Atualizar(livro);

        var usuario = await _usuarioRepository.FirstOrDefault(u => u.Id == emprestimo.UsuarioId);
        usuario!.QuantidadeEmprestimosRealizados -= 1;
        _usuarioRepository.Atualizar(usuario);

        emprestimo.DataDevolucaoRealizada = DateTime.Today;
        emprestimo.StatusEmprestimo = emprestimo.DataDevolucaoRealizada > emprestimo.DataDevolucaoPrevista
            ? EStatusEmprestimo.EntregueComAtraso
            : EStatusEmprestimo.Entregue;

        _emprestimoRepository.Atualizar(emprestimo);

        if (usuario.Bloqueado == true)
        {
            var emprestimoAtrasado = await _emprestimoRepository.FirstOrDefault(e =>
                e.UsuarioId == usuario.Id &&
                DateTime.Today > e.DataDevolucaoPrevista &&
                (e.StatusEmprestimo == EStatusEmprestimo.Emprestado ||
                 e.StatusEmprestimo == EStatusEmprestimo.Renovado));

            if (emprestimoAtrasado == null)
            {
                usuario.Bloqueado = false;
                _usuarioRepository.Atualizar(usuario);
            }
        }

        return await CommitChanges() ? Mapper.Map<EmprestimoDto>(emprestimo) : null;
    }

    public async Task<PaginacaoDto<EmprestimoDto>> Pesquisar(PesquisarEmprestimoDto dto)
    {
        var resultadoPaginado = await _emprestimoRepository.Pesquisar(dto.Id, dto.UsuarioId,
            dto.LivroId, dto.QuantidadeDeItensPorPagina, dto.PaginaAtual);

        return new PaginacaoDto<EmprestimoDto>
        {
            TotalDeItens = resultadoPaginado.TotalDeItens,
            QuantidadeDeItensPorPagina = resultadoPaginado.QuantidadeDeItensPorPagina,
            QuantidadeDePaginas = resultadoPaginado.QuantidadeDePaginas,
            PaginaAtual = resultadoPaginado.PaginaAtual,
            Itens = Mapper.Map<List<EmprestimoDto>>(resultadoPaginado.Itens)
        };
    }

    public async Task<List<EmprestimoDto>> ObterTodos()
    {
        var obterEmprestimos = await _emprestimoRepository.ObterTodos();
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