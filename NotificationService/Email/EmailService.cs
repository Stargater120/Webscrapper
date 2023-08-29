using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using MimeKit.Utils;
using NotificationService.Types;

namespace NotificationService.Email;

public class EmailService
{
    

    public void SendEmail(MimeMessage email)
    {
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