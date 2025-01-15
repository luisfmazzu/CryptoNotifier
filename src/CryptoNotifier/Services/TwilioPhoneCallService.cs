using System;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using System.Text;
using Twilio.TwiML.Messaging;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using System.Collections.Generic;

namespace CryptoNotifier.Services
{
    public class TwilioPhoneCallService : IPhoneCallService
    {
        private string accountSid;
        private string authToken;
        private string callNumberFromTwilio;

        public TwilioPhoneCallService(IConfiguration configuration)
        {
            accountSid = configuration["twilioAccountSID"];
            authToken = configuration["twilioAuthToken"];
            callNumberFromTwilio = configuration["twilioPhoneNumber"];

            TwilioClient.Init(accountSid, authToken);
        }

        public void SendVoiceCall(string name, double value, List<string> callNumberList, bool isPositiveCall)
        {
            StringBuilder message;

            if (isPositiveCall)
            {
                message = AssemblePositiveVoiceMessage(name, value);
            }
            else
            {
                message = AssembleNegativeVoiceMessage(name, value);
            }

            foreach (var callNumberTo in callNumberList)
            {
                var call = CallResource.Create(
                    to: new Twilio.Types.PhoneNumber(callNumberTo),
                    from: new Twilio.Types.PhoneNumber(callNumberFromTwilio),
                    twiml: message.ToString()
                );
            }
        }

        public StringBuilder AssembleNegativeVoiceMessage(string name, double value)
        {
            StringBuilder message = new StringBuilder();

            string spokenNumber = ConvertNumberToVoice(value);

            message.AppendLine("<Response>");
            message.AppendLine("<Say language='pt-BR'>");
            message.AppendLine("Olá " + name + ", tudo bem com você? Robôzinho do Luis aqui. ");
            message.AppendLine("A sua carteira de criptomoedas chegou no valor do limite de venda. Esse é um lembrete para você vender as moedas. ");
            message.AppendLine("O valor atual da sua carteira é de " + spokenNumber + " Tenha um ótimo dia e um abraço do Luisinho.");
            message.AppendLine("</Say>");
            message.AppendLine("</Response>");

            return message;
        }

        public StringBuilder AssemblePositiveVoiceMessage(string name, double value)
        {
            StringBuilder message = new StringBuilder();

            string spokenNumber = ConvertNumberToVoice(value);

            message.AppendLine("<Response>");
            message.AppendLine("<Say language='pt-BR'>");
            message.AppendLine("Olá " + name + ", tudo bem com você? Robôzinho do Luis aqui. ");
            message.AppendLine("Tenho uma ótima notícia para você. A sua carteira de criptomoedas chegou no seu valor mais alto até o momento.");
            message.AppendLine("O valor atual da sua carteira é de " + spokenNumber);
            message.AppendLine("Agora é hora de comemorar e tomar aquela gelada.");
            message.AppendLine("Tenha um ótimo dia e um abraço do Luisinho.");
            message.AppendLine("</Say>");
            message.AppendLine("</Response>");

            return message;
        }

        public string ConvertNumberToVoice(double value)
        {
            string spokenNumber = "";
            int intValue = Convert.ToInt32(value);

            if (intValue / 1000000 == 1)
            {
                spokenNumber += (intValue / 1000000).ToString("N0") + " milhão ";
                intValue -= 1000000;
            }
            else if (intValue / 1000000 > 0)
            {
                spokenNumber += (intValue / 1000000).ToString("N0") + " milhões ";
                intValue -= (intValue / 1000000) * 1000000;
            }
            if (intValue / 1000 > 0)
            {
                spokenNumber += (intValue / 1000).ToString("N0") + " mil ";
                intValue -= (intValue / 1000) * 1000;
            }
            if (intValue > 0)
            {
                spokenNumber += intValue.ToString("N0");
            }

            spokenNumber += " reais.";

            return spokenNumber;
        }

        public void SendCustomVoiceCall(List<string> callNumberList, StringBuilder message)
        {
            foreach (var callNumberTo in callNumberList)
            {
                var call = CallResource.Create(
                    to: new Twilio.Types.PhoneNumber(callNumberTo),
                    from: new Twilio.Types.PhoneNumber(callNumberFromTwilio),
                    twiml: message.ToString()
                );
            }
        }
    }
}
