using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using MimeKit.Utils;

namespace NotificationService.Email;

public class EmailService
{
    public void SendEmail()
    {
        // Dictionary<string, Dictionary<string, string>> sendDic
        var email = new MimeMessage();
        email.To.Add(new MailboxAddress("Tobias", "tobias.runge95@gmx.de"));
        email.From.Add(new MailboxAddress("Webscrapper", "stargater120@gmail.com"));
        email.Subject = "Webscrapping results";
        // var multipart = new Multipart("mixed");
        var builder = new BodyBuilder();
        var image = builder.LinkedResources.Add(Path.Combine(Directory.GetCurrentDirectory(), "testpic.jpg"));
        image.ContentId = MimeUtils.GenerateMessageId();
        builder.HtmlBody = $"<img src='cid:{image.ContentId}'>";
        builder.Attachments.Add(Path.Combine(Directory.GetCurrentDirectory(), "testpic.jpg"));
        email.Body = builder.ToMessageBody();
        
        using (var smtp = new SmtpClient())
        {
            smtp.Connect("smtp.gmail.com", 587, false);

            // Note: only needed if the SMTP server requires authentication
            smtp.Authenticate("stargater120@gmail.com", "ipqawvxlclmhselv");

            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}