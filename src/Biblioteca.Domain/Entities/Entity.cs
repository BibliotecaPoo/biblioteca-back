namespace Biblioteca.Domain.Entities;

public abstract class Entity
{
    public int Id { get; set; }
    public bool Ativo { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }
}