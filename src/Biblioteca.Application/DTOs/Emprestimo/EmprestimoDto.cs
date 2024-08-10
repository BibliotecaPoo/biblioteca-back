using Biblioteca.Application.DTOs.Livro;
using Biblioteca.Application.DTOs.Usuario;

namespace Biblioteca.Application.DTOs.Emprestimo;

public class EmprestimoDto
{
    public int Id { get; set; }
    public DateTime DataEmprestimo { get; set; }
    public DateTime DataDevolucaoPrevista { get; set; }
    public DateTime? DataDevolucaoRealizada { get; set; }
    public string StatusEmprestimo { get; set; } = null!;
    public int QuantidadeRenovacoesPermitida { get; set; }
    public int QuantidadeRenovacoesRealizadas { get; set; }
    public UsuarioDto Usuario { get; set; } = null!;
    public LivroDto Livro { get; set; } = null!;
}