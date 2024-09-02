namespace Biblioteca.Application.Email;

public class MailData
{
    public string AssuntoDoEmail { get; set; } = null!;
    public string MensagemDoEmail { get; set; } = null!;
    public string EmailParaSerEnviado { get; set; } = null!;
}