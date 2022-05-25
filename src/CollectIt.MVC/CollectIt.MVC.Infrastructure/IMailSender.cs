namespace CollectIt.MVC.Infrastructure;

public interface IMailSender
{
    Task SendMailAsync(string subject, string message, string to);
}