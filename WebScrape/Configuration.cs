using System.Configuration;
using WebScrape.Common;

namespace WebScrape
{
    public class Configuration : IConfiguration
    {
        public string StoragePath => ConfigurationManager.AppSettings.Get("StoragePath");
        public string CurrencyListUrl => ConfigurationManager.AppSettings.Get("CurrencyListUrl");
        public string RateUrl => ConfigurationManager.AppSettings.Get("RateUrl");
    }
}
