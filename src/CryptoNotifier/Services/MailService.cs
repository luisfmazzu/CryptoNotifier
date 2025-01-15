using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Collections.Generic;
using CryptoNotifier.Services;
using Microsoft.Extensions.Configuration;

namespace CryptoNotifier.Services
{
    public class MailSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string ServiceProviderName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class MailService: IMailService
    {
        SmtpClient _client;
        SmtpClient _announcementsClient;
        MailSettings _mailSettings;
        private const int JwtTokenIndexFromHeader = 1;
        private bool testEmailOnly = false;

        public MailService(IConfiguration configuration)
        {
            _mailSettings = new MailSettings()
            {
                Host = configuration["host"],
                Port = Convert.ToInt32(configuration["port"]),
                ServiceProviderName = configuration["serviceProviderName"],
                UserName = configuration["mailUserName"],
                Password = configuration["password"],
            };

            _client = new SmtpClient(_mailSettings.Host, _mailSettings.Port);
            _client.UseDefaultCredentials = false;
            _client.TargetName = _mailSettings.ServiceProviderName;
            _client.EnableSsl = true;
            _client.Credentials = new NetworkCredential(_mailSettings.UserName, _mailSettings.Password);
            _client.DeliveryMethod = SmtpDeliveryMethod.Network;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }

        public async Task SendMail(string subject, string message, HashSet<string> sendEmailList = null)
        {
            if(sendEmailList == null || testEmailOnly)
            {
                sendEmailList = new HashSet<string>()
                {
                    "luisfmazzu@gmail.com"
                };
            }
            foreach (string email in sendEmailList) 
            {
                try
                {
                    MailAddress from = new MailAddress(_mailSettings.UserName, String.Empty, Encoding.UTF8);
                    MailAddress to = new MailAddress(email);
                    MailMessage sendMessage = new MailMessage(from, to);
                    sendMessage.Body = message;
                    sendMessage.BodyEncoding = Encoding.UTF8;
                    sendMessage.Subject = subject;
                    sendMessage.SubjectEncoding = Encoding.UTF8;
                    sendMessage.IsBodyHtml = true;
                    await _client.SendMailAsync(sendMessage);
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}
