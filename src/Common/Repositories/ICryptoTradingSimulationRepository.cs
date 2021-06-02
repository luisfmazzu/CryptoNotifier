using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Domains;

namespace Common.Repositories
{
    public interface ICryptoTradingSimulationRepository
    {
        Task<ICryptoTradingSimulationDomain> GetCryptoTradingSimulationById(string cryptoTradingSimulationId);

        Task InsertCryptoTradingSimulation(ICryptoTradingSimulationDomain cryptoTradingSimulationDomain);

        Task UpdateCryptoTradingSimulation(string simulationId, ICryptoTradingSimulationDomain cryptoTradingSimulationDomain);

        Task DeleteCryptoTradingSimulation(string simulationId);
        Task<List<ICryptoTradingSimulationDomain>> GetAllCryptoTradingSimulation();
        Task<List<ICryptoTradingSimulationDomain>> GetAllFunctionalCryptoTradingSimulation();
    }
}