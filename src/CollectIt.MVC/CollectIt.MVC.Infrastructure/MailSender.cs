using System.Net.Mail;

namespace CollectIt.MVC.Infrastructure;

public class MailSender : IMailSender
{
    public MailSender(MailSenderOptions options)
    {
        Options = options;
    }

    public MailSenderOptions Options { get; }

    public async Task SendMailAsync(string subject, string msg, string to)
    {
        using var client = new SmtpClient();
        using var message = CreateMailMessage(subject, msg, to);
        await client.SendMailAsync(message);
    }

    public void SendMail(string subject, string msg, string to)
    {
        using var client = new SmtpClient();
        using var message = CreateMailMessage(subject, msg, to);
        client.SendAsync(message, null);
    }

    private MailMessage CreateMailMessage(string subject, string body, string to)
    {
        var message = new MailMessage()
                      {
                          Body = body, From = new MailAddress(Options.Username), Subject = subject, To = {to},
                      };
        return message;
    }
}