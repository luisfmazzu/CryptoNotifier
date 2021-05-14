using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Repositories;

namespace CryptoNotifier.Services
{
    public interface ICryptoAnalyzer
    {
        public void InitializeClient(ICryptoDataRepository cryptoDataRepository);
    }
}
