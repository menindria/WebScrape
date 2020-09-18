using System;
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
    public class RateProvider : IRateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public RateProvider(
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<IEnumerable<Domain.Models.Rate>> Execute(string currency, DateTime startDate, DateTime endDate)
        {
            IList<HtmlNode> rawDataRows = await GetRawHtmlRows(currency, startDate, endDate);

            return ParseRawData(rawDataRows);
        }

        private static List<Domain.Models.Rate> ParseRawData(IList<HtmlNode> rows)
        {
            var data = new List<Domain.Models.Rate>();
            foreach (HtmlNode row in rows)
            {
                List<HtmlNode> tds = row.QuerySelectorAll("td").ToList();

                data.Add(new Domain.Models.Rate()
                {
                    CurrencyName = tds[0].InnerText,
                    BuyingRate = tds[1].InnerText,
                    CashBuyingRate = tds[2].InnerText,
                    SellingRate = tds[3].InnerText,
                    CashSellingRate = tds[4].InnerText,
                    MiddleRate = tds[5].InnerText,
                    PubTime = tds[6].InnerText,
                });
            }

            return data;
        }

        private async Task<IList<HtmlNode>> GetRawHtmlRows(string currency, DateTime startDate, DateTime endDate)
        {
            var rawHtmlData = await GetRawHtmlData(currency, startDate, endDate);

            var document = new HtmlDocument();
            document.LoadHtml(rawHtmlData);

            IList<HtmlNode> trs = document
                .QuerySelectorAll("table")[2]
                .QuerySelectorAll("tr:not(:first-child)"); //Remove header row

            return trs;
        }

        private async Task<string> GetRawHtmlData(string currency, DateTime startDate, DateTime endDate)
        {
            var values = new Dictionary<string, string>
            {
                {"nothing", startDate.ToStandardDateString()},
                {"erectDate", endDate.ToStandardDateString()},
                {"pjname", currency},
            };

            var response = await _httpClient.PostAsync(
                _configuration.RateUrl,
                new FormUrlEncodedContent(values));

            return await response.Content.ReadAsStringAsync();
        }
    }
}