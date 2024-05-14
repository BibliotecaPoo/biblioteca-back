﻿namespace Biblioteca.Application.DTOs.Usuario;

public class UsuarioDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Matricula { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int QuantidadeEmprestimosPermitida { get; set; }
    public int QuantidadeEmprestimosRealizados { get; set; }
    public bool Bloqueado { get; set; }
    public int DiasBloqueado { get; set; }
    public DateTime? DataInicioBloqueio { get; set; }
    public DateTime? DataFimBloqueio { get; set; }
}