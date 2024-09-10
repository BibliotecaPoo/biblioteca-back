using System.Net;
using System.Net.Mail;
using Biblioteca.Application.Configurations;
using Biblioteca.Application.Notifications;
using Biblioteca.Domain.Entities;
using Microsoft.Extensions.Options;

namespace Biblioteca.Application.Email;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;

    public EmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }
    
    public async Task EnviarEmailParaRecuperarSenhaDoAdministrador(Administrador administrador)
    {
       // var url = $"{_appSettings.UrlComum}/resetar-senha?token={administrador.CodigoDeRecuperacaoDeSenha}";
        var body = 
            $"Olá {administrador.Nome},<br><br>" +
            "Você solicitou a alteração de senha para a sua conta na Biblioteca Acadêmica. Para continuar, copie e cole o código abaixo e siga as instruções para criar uma nova senha segura:<br><br>" +
            $"{administrador.CodigoDeRecuperacaoDeSenha}<br><br>" +
            "Se você não solicitou essa alteração, por favor, ignore este e-mail ou entre em contato conosco imediatamente.<br><br>" +
            "Atenciosamente, equipe Biblioteca Acadêmica";

        var mailData = new MailData
        {
            AssuntoDoEmail = "Redefina sua senha agora mesmo!",
            MensagemDoEmail = body,
            EmailParaSerEnviado = administrador.Email
        };
        
        await EnviarEmail(mailData);
    }

    public async Task EnviarEmail(MailData mailData)
    {
        var enviarPara = mailData.EmailParaSerEnviado;
        var usuario = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(_emailSettings.Usuario));
        var senha = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(_emailSettings.Senha));

        var smtpClient = new SmtpClient(_emailSettings.Servidor)
        {
            Port = _emailSettings.Porta,
            Credentials = new NetworkCredential(usuario, senha),
            EnableSsl = true,
        };

        var mensagem = new MailMessage(usuario, enviarPara)
        {
            Subject = mailData.AssuntoDoEmail,
            Body = mailData.MensagemDoEmail,
            IsBodyHtml = true
        };

        try
        {
            await Task.Run(() => smtpClient.Send(mensagem));
        }
        catch (Exception)
        {
            var notificator = new Notificator();
            notificator.Handle("Ocorreu um erro ao enviar o e-mail");

        }
    }
}