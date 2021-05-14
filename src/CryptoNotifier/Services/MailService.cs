using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CryptoNotifier.Services
{
    public class MailService : IMailService
    {
        private SmtpClient smtpClient;

        public MailService()
        {
            smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("cryptonotifierbros@gmail.com", "cryptonotifier123"),
                EnableSsl = true,
            };
        }

        public void Send(string subject, string message)
        {
            //Send email
            using (MailMessage mailMessage = new MailMessage("cryptonotifiersbros@gmail.com", "henrique.mazzu@gmail.com")
            {
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
                BodyEncoding = System.Text.Encoding.UTF8,

            })
            {
                smtpClient.Send(mailMessage);
            }
            using (MailMessage mailMessage = new MailMessage("cryptonotifiersbros@gmail.com", "luisfmazzu@gmail.com")
            {
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
                BodyEncoding = System.Text.Encoding.UTF8,

            })
            {
                smtpClient.Send(mailMessage);
            }
        }
    }
}
