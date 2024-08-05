namespace Biblioteca.Application.DTOs.Paginacao;

public class PaginacaoDto<T>
{
    public int TotalDeItens { get; set; }
    public int QuantidadeDeItensPorPagina { get; set; }
    public int QuantidadeDePaginas { get; set; }
    public int PaginaAtual { get; set; }
    public List<T> Itens { get; set; } = new();
}