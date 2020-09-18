using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using WebScrape.Application.Contracts;
using WebScrape.Common;
using WebScrape.Domain.Models;
using WebScrape.Infrastructure.RateAccess.Models;

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

        public async Task<IEnumerable<Rate>> Execute(string currency, DateTime startDate, DateTime endDate)
        {
            ItemsInfo itemsInfo = await GetItemsInfo(currency, startDate, endDate);

            if (!itemsInfo.HasItems)
            {
                return new List<Rate>();
            }

            //This can be optimized to not buffer all values, but to flush to file as soon as any of the tasks finish
            //but order in file will not be preserved

            var tasks = await Task.WhenAll(Enumerable.Range(1, itemsInfo.NumberOfPages)
                .Select((page) => Task.Run<IEnumerable<Rate>>(async () =>
                {
                    IList<HtmlNode> rawDataRows = await GetRawHtmlRows(currency, startDate, endDate, page);

                    return ParseRawData(rawDataRows);
                })));

            return tasks.SelectMany(x => x).ToList();
        }

        private static List<Rate> ParseRawData(IList<HtmlNode> rows)
        {
            var data = new List<Rate>();
            foreach (HtmlNode row in rows)
            {
                List<HtmlNode> tds = row.QuerySelectorAll("td").ToList();

                data.Add(new Rate()
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

        private async Task<IList<HtmlNode>> GetRawHtmlRows(string currency, DateTime startDate, DateTime endDate, int page)
        {
            string rawHtmlData = await GetRawHtmlData(currency, startDate, endDate, page);

            var document = new HtmlDocument();
            document.LoadHtml(rawHtmlData);

            IList<HtmlNode> trs = document
                .QuerySelectorAll("table")[2]
                .QuerySelectorAll("tr:not(:first-child)"); //Remove header row

            return trs;
        }

        private async Task<ItemsInfo> GetItemsInfo(string currency, DateTime startDate, DateTime endDate)
        {
            string rawHtmlData = await GetRawHtmlData(currency, startDate, endDate, 1);

            var document = new HtmlDocument();
            document.LoadHtml(rawHtmlData);

            var itemsInfo = new ItemsInfo();
            if (!rawHtmlData.Contains("sorry, no records！"))
            {
                itemsInfo.Count = Parse(rawHtmlData, "m_nRecordCount");
                itemsInfo.PageSize = Parse(rawHtmlData, "m_nPageSize");
            }

            return itemsInfo;
        }

        private static int Parse(string text, string variableName)
        {
            var line = Regex.Match(text, $".*var {variableName}.*;").ToString();
            return int.Parse(Regex.Replace(line, "[^0-9.]", String.Empty));
        }

        private async Task<string> GetRawHtmlData(string currency, DateTime startDate, DateTime endDate, int page)
        {
            var values = new Dictionary<string, string>
            {
                {"nothing", startDate.ToStandardDateString()},
                {"erectDate", endDate.ToStandardDateString()},
                {"pjname", currency},
                {"page", page.ToString()},
            };

            var response = await _httpClient.PostAsync(
                _configuration.RateUrl,
                new FormUrlEncodedContent(values));

            return await response.Content.ReadAsStringAsync();
        }
    }
}