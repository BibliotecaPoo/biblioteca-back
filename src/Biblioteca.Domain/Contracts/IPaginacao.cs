using Biblioteca.Domain.Entities;

namespace Biblioteca.Domain.Contracts;

public interface IPaginacao<T> where T : Entity, new()
{
    public int TotalDeItens { get; set; }
    public int QuantidadeDeItensPorPagina { get; set; }
    public int QuantidadeDePaginas { get; set; }
    public int PaginaAtual { get; set; }
    public List<T> Itens { get; set; }
}