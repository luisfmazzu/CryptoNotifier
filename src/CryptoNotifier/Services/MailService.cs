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
            int tryAgain = 10;
            bool failed = false;

            do
            {
                try
                {
                    failed = false;

                    var smtp = new SmtpClient("smtp.gmail.com")
                    {
                        Port = 587,
                        Credentials = new NetworkCredential("cryptonotifierbros@gmail.com", "cryptonotifier123"),
                        EnableSsl = true,
                    };
                    var mail = new MailMessage("cryptonotifiersbros@gmail.com", "henrique.mazzu@gmail.com");
                    mail.Subject = subject;
                    mail.Body = message;
                    mail.IsBodyHtml = true;
                    smtp.Send(mail);
                }
                catch (Exception ex)
                {
                    failed = true;
                    tryAgain--;
                    var exception = ex.Message.ToString();
                }
            } while (failed && tryAgain != 0);

            tryAgain = 10;
            failed = false;

            do
            {
                try
                {
                    failed = false;

                    var smtp = new SmtpClient("smtp.gmail.com")
                    {
                        Port = 587,
                        Credentials = new NetworkCredential("cryptonotifierbros@gmail.com", "cryptonotifier123"),
                        EnableSsl = true,
                    };
                    var mail = new MailMessage("cryptonotifiersbros@gmail.com", "luisfmazzu@gmail.com");
                    mail.Subject = subject;
                    mail.Body = message;
                    mail.IsBodyHtml = true;
                    smtp.Send(mail);
                }
                catch (Exception ex)
                {
                    failed = true;
                    tryAgain--;
                    var exception = ex.Message.ToString();
                }
            } while (failed && tryAgain != 0);
        }
    }
}
