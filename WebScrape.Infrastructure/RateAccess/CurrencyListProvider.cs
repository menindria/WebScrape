using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using WebScrape.Application.Contracts;
using WebScrape.Common;

namespace WebScrape.Infrastructure.RateAccess
{
    public class CurrencyListProvider : ICurrencyListProvider
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public CurrencyListProvider(
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<IEnumerable<string>> Execute()
        {
            var response = await _httpClient.GetAsync( _configuration.CurrencyListUrl);

            HtmlDocument doc = new HtmlDocument();
            doc.Load(await response.Content.ReadAsStreamAsync());

            return doc.QuerySelectorAll("[name='pjname'] > option")
                    .Where(x => x.InnerText != "Select the currency")
                    .Select(x => x.InnerText)
                    .ToList();

        }
    }
}
