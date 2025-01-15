using Common.Domains;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.Repositories
{
    public interface IWalletAlertsRepository
    {
        Task<WalletAlertsDto> GetWalletAlertsDtoByName(string walletName);
        Task<List<WalletAlertsDto>> GetAllWalletAlertsDto();

        Task InsertWalletAlertsDto(WalletAlertsDto walletAlertsDto);

        Task UpdateWalletAlertsDto(WalletAlertsDto walletAlertsDto);

        Task DeleteWalletAlertsDto(string walletName);
    }
}
