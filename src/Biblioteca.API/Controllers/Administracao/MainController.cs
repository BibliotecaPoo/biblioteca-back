using Biblioteca.Application.Notifications;
using Biblioteca.Core.Authorization;
using Biblioteca.Core.Enum;

namespace Biblioteca.API.Controllers.Administracao;

[ClaimsAuthorize("TipoUsuario", ETipoUsuario.Administrador)]
public abstract class MainController : BaseController
{
    protected MainController(INotificator notificator) : base(notificator)
    {
    }
}