using System.ComponentModel;

namespace Biblioteca.Core.Enum;

public enum ETipoUsuario
{
    [Description("Administrador")]
    Administrador = 1,
    
    [Description("Comum")]
    Comum = 2
}