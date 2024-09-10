namespace Biblioteca.Domain.Entities;

public class Administrador : Pessoa
{
    public string? CodigoDeRecuperacaoDeSenha { get; set; }
    public DateTime? TempoDeExpiracaoDoCodigoDeRecuperacaoDeSenha { get; set; }
    public bool PedidoDeRecuperacaoDeSenha { get; set; }
}