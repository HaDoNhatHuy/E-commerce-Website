using System.Net.Mail;
using System.Net;

namespace Web.Areas.Admin.Repository
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true, //bật bảo mật
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("nhathuy.hado@gmail.com", "vldzfqjcwuzdtdbl")
            };

            return client.SendMailAsync(
                new MailMessage(from: "nhathuy.hado@gmail.com",
                                to: email,
                                subject,
                                message
                                ));
        }
    }
}

