using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using MonitoramentoAPI.Models;

namespace MonitoramentoAPI.Services
{
    public class EmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task EnviarEmailFalha(string emailDestino, string nomeServico, string erro)
        {
            var mensagem = new MailMessage
            {
                From = new MailAddress(_smtpSettings.From),
                Subject = $"[Monitoramento] Falha no serviço: {nomeServico}",
                Body = $"O serviço {nomeServico} apresentou uma falha:\n\n{erro}",
                IsBodyHtml = false
            };

            mensagem.To.Add(emailDestino);

            using var smtp = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
            {
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                EnableSsl = _smtpSettings.EnableSsl
            };

            await smtp.SendMailAsync(mensagem);
        }
    }
}