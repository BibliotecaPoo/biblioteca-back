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
        if (!await ValidacoesParaRealizarEmprestimo(dto))
            return null;

        var usuario = await _usuarioRepository.FirstOrDefault(u => u.Matricula == dto.UsuarioMatricula);
        var livro = await _livroRepository.FirstOrDefault(l => l.Codigo == dto.LivroCodigo);

        var resultadoVerificacaoSenha = _passwordHasher.VerifyHashedPassword(usuario!, usuario!.Senha, dto.UsuarioSenha);
        if (resultadoVerificacaoSenha == PasswordVerificationResult.Failed)
        {
            Notificator.Handle("Senha incorreta.");
            return null;
        }

        livro!.QuantidadeExemplaresDisponiveisParaEmprestimo -= 1;
        if (livro.QuantidadeExemplaresDisponiveisParaEmprestimo == 0)
        {
            livro.StatusLivro = EStatusLivro.Indisponivel;
        }
        _livroRepository.Atualizar(livro);

        usuario.QuantidadeEmprestimosRealizados += 1;
        _usuarioRepository.Atualizar(usuario);

        var emprestimo = new Emprestimo();
        emprestimo.DataEmprestimo = DateTime.Today;
        emprestimo.DataDevolucaoPrevista = DateTime.Today.AddDays(10);
        emprestimo.StatusEmprestimo = EStatusEmprestimo.Emprestado;
        emprestimo.QuantidadeRenovacoesPermitida = 5;
        emprestimo.UsuarioId = usuario.Id;
        emprestimo.LivroId = livro.Id;

        _emprestimoRepository.Adicionar(emprestimo);
        return await CommitChanges() ? Mapper.Map<EmprestimoDto>(emprestimo) : null;
    }

    public async Task<EmprestimoDto?> RealizarRenovacao(int id, RealizarRenovacaoDto dto)
    {
        if (!await ValidacoesParaRealizarRenovacao(id, dto))
            return null;

        var emprestimo = await _emprestimoRepository.FirstOrDefault(e => e.Id == id);
        var usuario = await _usuarioRepository.FirstOrDefault(u => u.Matricula == dto.UsuarioMatricula);

        var resultadoVerificacaoSenha = _passwordHasher.VerifyHashedPassword(usuario!, usuario!.Senha, dto.UsuarioSenha);
        if (resultadoVerificacaoSenha == PasswordVerificationResult.Failed)
        {
            Notificator.Handle("Senha incorreta.");
            return null;
        }

        emprestimo!.DataEmprestimo = DateTime.Today;
        emprestimo.DataDevolucaoPrevista = DateTime.Today.AddDays(10);
        emprestimo.StatusEmprestimo = EStatusEmprestimo.Renovado;
        emprestimo.QuantidadeRenovacoesRealizadas += 1;

        _emprestimoRepository.Atualizar(emprestimo);
        return await CommitChanges() ? Mapper.Map<EmprestimoDto>(emprestimo) : null;
    }

    public async Task<EmprestimoDto?> RealizarEntrega(int id, RealizarEntregaDto dto)
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

        if (emprestimo.StatusEmprestimo is EStatusEmprestimo.Entregue or EStatusEmprestimo.EntregueComAtraso)
        {
            Notificator.Handle("O empréstimo já está como entregue.");
            return null;
        }

        var livro = await _livroRepository.FirstOrDefault(l => l.Codigo == dto.LivroCodigo);
        if (livro!.StatusLivro == EStatusLivro.Indisponivel)
        {
            livro.StatusLivro = EStatusLivro.Disponivel;
        }
        livro.QuantidadeExemplaresDisponiveisParaEmprestimo += 1;
        _livroRepository.Atualizar(livro);

        var usuario = await _usuarioRepository.FirstOrDefault(u => u.Matricula == dto.UsuarioMatricula);
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
            dto.UsuarioMatricula, dto.LivroId, dto.LivroCodigo, dto.QuantidadeDeItensPorPagina, dto.PaginaAtual);

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

    private async Task<bool> ValidacoesParaRealizarEmprestimo(RealizarEmprestimoDto dto)
    {
        var usuario = await _usuarioRepository.FirstOrDefault(u => u.Matricula == dto.UsuarioMatricula);
        if (usuario == null)
        {
            Notificator.Handle("Usuário não encontrado com a matrícula informada.");
            return false;
        }

        if (usuario.Bloqueado == true)
        {
            Notificator.Handle("O usuário está temporariamente impedido de realizar empréstimos ou renovações.");
            return false;
        }

        var emprestimoAtrasado = await _emprestimoRepository.FirstOrDefault(e =>
            e.UsuarioId == usuario.Id &&
            DateTime.Today > e.DataDevolucaoPrevista &&
            (e.StatusEmprestimo == EStatusEmprestimo.Emprestado || e.StatusEmprestimo == EStatusEmprestimo.Renovado));
        if (emprestimoAtrasado != null)
        {
            usuario.Bloqueado = true;
            _usuarioRepository.Atualizar(usuario);

            Notificator.Handle("O usuário possui livro(s) não devolvido(s) e atrasado(s). " +
                               "O usuário será impedido de fazer empréstimos/renovações até devolvê-lo(s).");

            return false;
        }

        var livro = await _livroRepository.FirstOrDefault(l => l.Codigo == dto.LivroCodigo);
        if (livro == null)
        {
            Notificator.Handle("Livro não encontrado com o código informado.");
            return false;
        }

        var emprestimoIgual = await _emprestimoRepository.FirstOrDefault(e =>
            e.LivroId == livro.Id &&
            e.UsuarioId == usuario.Id &&
            (e.StatusEmprestimo == EStatusEmprestimo.Emprestado || e.StatusEmprestimo == EStatusEmprestimo.Renovado));
        if (emprestimoIgual != null)
        {
            Notificator.Handle("No momento o usuário já possui um exemplar emprestado ou renovado desse mesmo livro.");
            return false;
        }

        if (usuario.QuantidadeEmprestimosRealizados == usuario.QuantidadeEmprestimosPermitida)
        {
            Notificator.Handle("O usuário já atingiu o limite de empréstimos.");
            return false;
        }

        if (livro.StatusLivro == EStatusLivro.Indisponivel)
        {
            Notificator.Handle("Não existe exemplar disponível no momento para esse livro.");
            return false;
        }

        return true;
    }

    private async Task<bool> ValidacoesParaRealizarRenovacao(int id, RealizarRenovacaoDto dto)
    {
        if (id != dto.Id)
        {
            Notificator.Handle("O id informado na url deve ser igual ao id informado no json.");
            return false;
        }

        var emprestimo = await _emprestimoRepository.FirstOrDefault(e => e.Id == id);
        if (emprestimo == null)
        {
            Notificator.HandleNotFoundResource();
            return false;
        }

        var usuario = await _usuarioRepository.FirstOrDefault(u => u.Matricula == dto.UsuarioMatricula);
        if (usuario!.Bloqueado == true)
        {
            Notificator.Handle("O usuário está temporariamente impedido de realizar empréstimos ou renovações.");
            return false;
        }

        var emprestimoAtrasado = await _emprestimoRepository.FirstOrDefault(e =>
            e.UsuarioId == usuario.Id &&
            DateTime.Today > e.DataDevolucaoPrevista &&
            (e.StatusEmprestimo == EStatusEmprestimo.Emprestado || e.StatusEmprestimo == EStatusEmprestimo.Renovado));
        if (emprestimoAtrasado != null)
        {
            usuario.Bloqueado = true;
            _usuarioRepository.Atualizar(usuario);

            Notificator.Handle("O usuário possui livro(s) não devolvido(s) e atrasado(s). " +
                               "O usuário será impedido de fazer empréstimos/renovações até devolvê-lo(s).");

            return false;
        }

        if (emprestimo.QuantidadeRenovacoesRealizadas == emprestimo.QuantidadeRenovacoesPermitida)
        {
            Notificator.Handle("O usuário já atingiu o limite de renovações para esse livro.");
            return false;
        }

        if (emprestimo.StatusEmprestimo is EStatusEmprestimo.Entregue or EStatusEmprestimo.EntregueComAtraso)
        {
            Notificator.Handle("O empréstimo já está como entregue. Não é possível renovar o livro.");
            return false;
        }

        return true;
    }

    private async Task<bool> CommitChanges()
    {
        if (await _emprestimoRepository.UnitOfWork.Commit())
            return true;

        Notificator.Handle("Ocorreu um erro ao salvar as alterações.");
        return false;
    }
}