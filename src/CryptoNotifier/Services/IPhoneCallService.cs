using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CryptoNotifier.Services
{
    public interface IPhoneCallService
    {
        public void SendVoiceCall(string name, double value, List<string> callNumberList, bool isPositiveCall);
        public StringBuilder AssembleNegativeVoiceMessage(string name, double value);
        public StringBuilder AssemblePositiveVoiceMessage(string name, double value);
        public string ConvertNumberToVoice(double value);
        public void SendCustomVoiceCall(List<string> callNumberList, StringBuilder message);
    }
}
