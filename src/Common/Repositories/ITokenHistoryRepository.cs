using Common.Domains;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.Repositories
{
    public interface ITokenHistoryRepository
    {
        Task<TokenHistory> GetTokenHistoryBySymbol(string symbol);
        Task<List<TokenHistory>> GetAllTokenHistoryDto();

        Task InsertTokenHistory(TokenHistory tokenHistory);

        Task UpdateTokenHistory(TokenHistory tokenHistory);

        Task DeleteTokenHistory(string tickerSymbol);
    }
}
