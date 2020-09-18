using System.Net.Http;
using WebScrape.Application.Contracts;
using WebScrape.Common;

namespace WebScrape.Infrastructure.RateAccess
{
    public class RateProviderFactory : IRateProviderFactory
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public RateProviderFactory(
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public IRateProvider Create()
        {
            return new RateProvider(_httpClient, _configuration);
        }
    }
}
