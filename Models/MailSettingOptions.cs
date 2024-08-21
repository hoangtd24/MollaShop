namespace Molla.Models;
public class MailSettingOptions
{
    public const string MailSettings = "MailSettings";
    public string MailServer { set; get; } = string.Empty;
    public int MailPort { set; get; }
    public string SenderName { set; get; } = string.Empty;
    public string FromEmail { set; get; } = string.Empty;
    public string Password { set; get; } = string.Empty;

}