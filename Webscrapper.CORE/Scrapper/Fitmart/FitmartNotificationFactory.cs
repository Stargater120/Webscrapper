using MimeKit;
using MimeKit.Utils;
using NotificationService.Email;
using NotificationService.Types;

namespace Webscrapper.CORE.Fitmart;

public class FitmartNotificationFactory
{
    private readonly EmailService _emailService = new EmailService();
    public void CreateFitmartEMail(FitmartEmailNotification notificationItems)
    {
        // Dictionary<string, Dictionary<string, string>> sendDic
        var email = new MimeMessage();
        email.To.Add(new MailboxAddress("Tobias", "tobias.runge95@gmx.de"));
        email.From.Add(new MailboxAddress("Webscrapper", "stargater120@gmail.com"));
        email.Subject = "Webscrapping results";
        var builder = new BodyBuilder();
        var htmlBody = "<h2>Banners</h2><br>";
        var nameCounter = 1;
        foreach (var bytes in notificationItems.Banner)
        {
            var image = builder.LinkedResources.Add($"banner{nameCounter}.jpg", bytes);
            image.ContentId = MimeUtils.GenerateMessageId();
            htmlBody += $"<img src='cid:{image.ContentId}' style='height:150px; width:400px;'> <br>";
            // builder.Attachments.Add($"banner{nameCounter}", bytes);
            nameCounter++;
        }

        htmlBody += "<h2>Product's</h2><br>";
        foreach (var item in notificationItems.Items)
        {
            htmlBody += $"<p>{item.Name}:  </p><p>{item.OldPrice} --></p><p>{item.NewPrice}</p><br>";
        }

        builder.HtmlBody = htmlBody;
        email.Body = builder.ToMessageBody();
        
        _emailService.SendEmail(email);
    }
}