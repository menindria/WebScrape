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

            //Here we can use dependency injection container, I picked up poor man's dependency injection 
            IConfiguration configuration = new Configuration();
            Common.ILogger logger = new Logger(new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("log.txt")
                .CreateLogger());
            IUILogger uiLogger = new UILogger();

            RegisterGlobalExceptionHandler(logger);
            InitializeStorage(configuration);

            using (var httpClient = new HttpClient(new HttpRetryMessageHandler(new HttpClientHandler(), logger)))
            {
                IJob job = new Job(
                    new CurrencyListProvider(httpClient, configuration),
                    new RateProviderFactory(httpClient, configuration),
                    new RepositoryFactory(configuration.StoragePath),
                    uiLogger,
                    logger);

                uiLogger.Message("Starting process.");
                await job.Execute(DateTime.Now.AddDays(-1), DateTime.Now);
            }

            Console.WriteLine("Process done.");
            Console.WriteLine("Press any key to close.");
            Console.Read();
        }

        private static void InitializeStorage(IConfiguration configuration)
        {
            new StorageInitializer().Initialize(configuration.StoragePath);
        }

        private static void RegisterGlobalExceptionHandler(Common.ILogger logger)
        {
            AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs e) =>
            {
                logger.LogError((Exception) e.ExceptionObject);
            };
        }
    }
}
