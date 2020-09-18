using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Serilog;
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

            IConfiguration configuration = new Configuration();
            var logger = new Logger(new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("log.txt")
                .CreateLogger());

            RegisterGlobalExceptionHandler(logger);
            InitializeStorage(configuration);

            using (var httpClient = new HttpClient())
            {
                IJob job = new Job(
                    new CurrencyListProvider(httpClient, configuration),
                    new RateProviderFactory(httpClient, configuration),
                    new RepositoryFactory(configuration.StoragePath),
                    new UILogger(),
                    logger);


                await job.Execute(DateTime.UtcNow.AddDays(-2), DateTime.UtcNow);
            }

            Console.WriteLine("Work done.");
            Console.WriteLine("Press any key to close.");
            Console.Read();
        }

        private static void InitializeStorage(IConfiguration configuration)
        {
            new StorageInitializer().Initialize(configuration.StoragePath);
        }

        private static void RegisterGlobalExceptionHandler(Logger logger)
        {
            AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs e) =>
            {
                logger.LogException((Exception) e.ExceptionObject);
            };
        }
    }
}
