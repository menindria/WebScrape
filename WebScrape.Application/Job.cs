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
        private readonly IUILogger _iuiLogger;
        private readonly ILogger _logger;

        public Job(
            ICurrencyListProvider currencyListProvider,
            IRateProviderFactory rateProviderFactory,
            IRepositoryFactory repositoryFactory,
            IUILogger iuiLogger,
            ILogger logger)
        {
            _currencyListProvider = currencyListProvider;
            _rateProviderFactory = rateProviderFactory;
            _repositoryFactory = repositoryFactory;
            _iuiLogger = iuiLogger;
            _logger = logger;
        }

        public async Task Execute(DateTime startDate, DateTime endDate)
        {
            IEnumerable<string> currencyList = await _currencyListProvider.Execute();

            //It is possible also to add retry logic, but it was not in requirements

            await Task.WhenAll(currencyList.Select(currency => Task.Run(async () => //Here we can also use Parallel.ForEach that is not thread optimized but faster
            {
                try
                {
                    IEnumerable<Rate> data = await _rateProviderFactory.Create().Execute(currency, startDate, endDate);

                    var fileName = $"{currency} {startDate.ToStandardDateString()}-{endDate.ToStandardDateString()}";
                    _repositoryFactory.Create().Save(data, fileName);
                }
                catch (Exception exception)
                {
                    _logger.LogException(exception);
                    _iuiLogger.FailedScraping($"Failed for currency {currency}");
                }
            })));
        }
    }
}
