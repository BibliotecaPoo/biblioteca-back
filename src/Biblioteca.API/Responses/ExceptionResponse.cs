using System.Net;

namespace Biblioteca.API.Responses;

public class ExceptionResponse : Response
{
    public ExceptionResponse()
    {
        Title = "Ocorreu um erro no servidor.";
        Status = (int)HttpStatusCode.InternalServerError;
    }

    public ExceptionResponse(string title) : this()
    {
        Title = title;
    }
}