using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoNotifier.Services
{
    public interface IMailService
    {
        Task SendMail(string subject, string message, HashSet<string> sendEmailList = null);
    }
}
