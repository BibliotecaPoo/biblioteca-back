using System.Net;
using System.Text.Json.Serialization;

namespace Biblioteca.API.Responses;

public class BadRequestResponse : Response
{
    [JsonPropertyOrder(order: 3)] 
    public List<string>? Errors { get; private set; }

    public BadRequestResponse()
    {
        Title = "Ocorreram um ou mais errors de validação.";
        Status = (int)HttpStatusCode.BadRequest;
    }

    public BadRequestResponse(List<string>? errors) : this()
    {
        Errors = errors ?? new List<string>();
    }
}