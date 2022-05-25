using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;

namespace CollectIt.MVC.Infrastructure;

public class MailSender : IMailSender
{
    private readonly ILogger<MailSender> _logger;

    public MailSender(MailSenderOptions options, ILogger<MailSender> logger)
    {
        _logger = logger;
        Options = options;
    }

    public MailSenderOptions Options { get; }

    public async Task SendMailAsync(string subject, string msg, string to)
    {
        using var client = new SmtpClient();
        using var message = CreateMailMessage(subject, msg, to);
        try
        {
            await client.ConnectAsync(Options.Host, Options.Port, Options.EnableSsl);
            await client.AuthenticateAsync(Options.Username, Options.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while sending email to address '{Address}'", to);
        }
    }


    private MimeMessage CreateMailMessage(string subject, string body, string to)
    {
        var message = new MimeMessage()
                      {
                          Body = new TextPart(TextFormat.Html) {Text = body},
                          From = {new MailboxAddress("CollectIt System", Options.From)},
                          Subject = subject,
                          To = {new MailboxAddress(string.Empty, to)},
                      };
        return message;
    }
}