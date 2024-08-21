using System;
using System.Threading.Tasks;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Molla.Models;

namespace Molla.Services;

public class SendMailService : ISendMailService
{
    private readonly MailSettingOptions mailSettings;

    private readonly ILogger<SendMailService> logger;


    // mailSetting được Inject qua dịch vụ hệ thống
    // Có inject Logger để xuất log
    public SendMailService(IOptions<MailSettingOptions> _mailSettings, ILogger<SendMailService> _logger)
    {
        mailSettings = _mailSettings.Value;
        logger = _logger;
        logger.LogInformation("Create SendMailService");
    }

    // Gửi email, theo nội dung trong mailContent
    public async Task SendMail(MailContent mailContent)
    {
        var email = new MimeMessage();
        email.Sender = new MailboxAddress(mailSettings.FromEmail, mailSettings.FromEmail);
        email.From.Add(new MailboxAddress(mailSettings.FromEmail, mailSettings.FromEmail));
        email.To.Add(MailboxAddress.Parse(mailContent.To));
        email.Subject = mailContent.Subject;


        var builder = new BodyBuilder();
        builder.HtmlBody = mailContent.Body;
        email.Body = builder.ToMessageBody();

        // dùng SmtpClient của MailKit
        using var smtp = new MailKit.Net.Smtp.SmtpClient();

        try
        {
            Console.WriteLine(mailSettings.MailPort);
            smtp.Connect(mailSettings.MailServer, mailSettings.MailPort, SecureSocketOptions.StartTls);
            smtp.Authenticate(mailSettings.FromEmail, mailSettings.Password);
            await smtp.SendAsync(email);
        }
        catch (Exception ex)
        {
            // Gửi mail thất bại, nội dung email sẽ lưu vào thư mục mailssave
            System.IO.Directory.CreateDirectory("mailssave");
            var emailsavefile = string.Format(@"mailssave/{0}.eml", Guid.NewGuid());
            await email.WriteToAsync(emailsavefile);

            logger.LogInformation("Lỗi gửi mail, lưu tại - " + emailsavefile);
            logger.LogError(ex.Message);
        }

        smtp.Disconnect(true);

        logger.LogInformation("send mail to " + mailContent.To);

    }
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        Console.WriteLine("emailsavefile sssssss");
        await SendMail(new MailContent()
        {
            To = email,
            Subject = subject,
            Body = htmlMessage
        });
    }
}

public interface ISendMailService
{
    Task SendMail(MailContent mailContent);
    Task SendEmailAsync(string email, string subject, string htmlMessage);
}

public class MailContent
{
    public string To { get; set; } = String.Empty;          // Địa chỉ gửi đến
    public string Subject { get; set; } = String.Empty;    // Chủ đề (tiêu đề email)
    public string Body { get; set; } = String.Empty;       // Nội dung (hỗ trợ HTML) của email

}