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
        private readonly IStatusLogger _statusLogger;

        public Job(
            ICurrencyListProvider currencyListProvider,
            IRateProviderFactory rateProviderFactory,
            IRepositoryFactory repositoryFactory,
            IStatusLogger statusLogger)
        {
            _currencyListProvider = currencyListProvider;
            _rateProviderFactory = rateProviderFactory;
            _repositoryFactory = repositoryFactory;
            _statusLogger = statusLogger;
        }

        public async Task Execute(DateTime startDate, DateTime endDate)
        {
            IEnumerable<string> currencyList = await _currencyListProvider.Execute();


            await Task.WhenAll(currencyList.Select(currency => Task.Run(async () => //Here we can also use Parallel.ForEach that is not thread optimzied
            {
                try
                {
                    IEnumerable<Rate> data = await _rateProviderFactory.Create().Execute(currency, startDate, endDate);

                    var fileName = $"{currency} {startDate.ToStandardDateString()}-{endDate.ToStandardDateString()}";
                    _repositoryFactory.Create().Save(data, fileName);
                }
                catch
                {
                    _statusLogger.FailedScraping($"Failed for currency {currency}");
                    //Missing exception logger, it was not required by requirements
                }
            })));
        }
    }
}
