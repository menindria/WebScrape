using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebScrape.Application;
using WebScrape.Application.Contracts;
using WebScrape.Common;
using WebScrape.Infrastructure.Logger;
using WebScrape.Infrastructure.RateAccess;
using WebScrape.Infrastructure.Storage;

namespace WebScrape
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var httpClient = new HttpClient();
            IConfiguration configuration = new Configuration();
            new StorageInitializer().Initialize(configuration.StoragePath);

            IJob job = new Job(
                new CurrencyListProvider(httpClient, configuration), 
                new RateProviderFactory(httpClient, configuration), 
                new RepositoryFactory(configuration.StoragePath),
                new StatusLogger());


            await job.Execute(DateTime.UtcNow.AddDays(-2), DateTime.UtcNow);

            Console.WriteLine("Work done.");
            Console.WriteLine("Press any key to close.");
            Console.Read();
        }
    }
}
