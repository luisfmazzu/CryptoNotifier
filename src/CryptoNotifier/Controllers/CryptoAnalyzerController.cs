using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Common.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using CryptoNotifier.Services;

namespace CryptoNotifier.Controllers
{
        [ApiController]
        [Route("api/currentfii")]
        public class CryptoAnalyzerController : ControllerBase
        {
            private ILogger<CryptoAnalyzerController> _logger;

            private readonly ICryptoDataRepository _cryptoDataRepository;
            private readonly ICryptoAnalyzer _cryptoAnalyzer;

            public CryptoAnalyzerController(ILogger<CryptoAnalyzerController> logger, ICryptoDataRepository cryptoDataPersistenceService, ICryptoAnalyzer cryptoAnalyzer)
            {
                _logger = logger;
                _cryptoDataRepository = cryptoDataPersistenceService;
                _cryptoAnalyzer = cryptoAnalyzer;
                _cryptoAnalyzer.InitializeClient();
            }
        }
}
