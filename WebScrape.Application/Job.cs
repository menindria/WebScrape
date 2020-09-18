using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebScrape.Application.Contracts;
using WebScrape.Common;
using WebScrape.Domain.Models;

namespace WebScrape.Application
{
    public class Job : IJob
    {
        private readonly ICurrencyListProvider _currencyListProvider;
        private readonly IRateProviderFactory _rateProviderFactory;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IUILogger _uiLogger;
        private readonly ILogger _logger;

        public Job(
            ICurrencyListProvider currencyListProvider,
            IRateProviderFactory rateProviderFactory,
            IRepositoryFactory repositoryFactory,
            IUILogger uiLogger,
            ILogger logger)
        {
            _currencyListProvider = currencyListProvider;
            _rateProviderFactory = rateProviderFactory;
            _repositoryFactory = repositoryFactory;
            _uiLogger = uiLogger;
            _logger = logger;
        }

        public async Task Execute(DateTime startDate, DateTime endDate)
        {
            IEnumerable<string> currencyList = await _currencyListProvider.Execute();

            _uiLogger.Message($"Number of currencies: {currencyList.Count()}");

            await Task.WhenAll(currencyList.Select(currency => Task.Run(async () => //Here we can also use Parallel.ForEach that is not thread optimized but faster
            {
                try
                {
                    IEnumerable<Rate> data = await _rateProviderFactory.Create().Execute(currency, startDate, endDate);

                    var fileName = $"{currency} {startDate.ToStandardDateString()}-{endDate.ToStandardDateString()}";
                    _repositoryFactory.Create().Save(data, fileName);
                    _uiLogger.Message($"Finished for currency {currency}, number of results: {data.Count()}");
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception);
                    _uiLogger.Message($"Failed for currency {currency}");
                }
            })));
        }
    }
}
