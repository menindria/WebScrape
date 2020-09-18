using System;
using Serilog;

namespace WebScrape.Infrastructure.Logger
{
    public class Logger : Common.ILogger
    {
        private readonly ILogger _logger;

        public Logger(ILogger logger)
        {
            _logger = logger;
        }

        public void LogError(Exception exception)
        {
            _logger.Error(exception, exception.Message);
        }
        
        public void LogError(string message)
        {
            _logger.Error(message);
        }
    }
}
