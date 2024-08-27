using System.Net;
using Newtonsoft.Json;

namespace Biblioteca.API.Responses;

public class BadRequestResponse : Response
{
    [JsonProperty(Order = 3)] public List<string>? Errors { get; private set; }

    public BadRequestResponse(List<string>? errors)
    {
        Title = "Ocorreram um ou mais errors de validação.";
        Status = (int)HttpStatusCode.BadRequest;
        Errors = errors ?? new List<string>();
    }
}