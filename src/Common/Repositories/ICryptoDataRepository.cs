using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Domains;

namespace Common.Repositories
{
    public interface ICryptoDataRepository
    {
        Task<ICryptoDataDomain> GetCryptoDataById(string cryptoDataId);

        Task<ICryptoDataDomain> GetCryptoDataByTickerSymbol(string tickerSymbol);

        Task InsertCryptoData(ICryptoDataDomain cryptoDataDomain);

        Task UpdateCryptoData(string tickerSymbol, ICryptoDataDomain cryptoDataDomain);

        Task DeleteCryptoData(string tickerSymbol);
        Task<List<ICryptoDataDomain>> GetAllCryptoDatas();
    }
}
