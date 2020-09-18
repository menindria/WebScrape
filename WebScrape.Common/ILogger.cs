using System;

namespace WebScrape.Common
{
    public interface ILogger
    {
        void LogError(Exception exception);
        void LogError(string message);
    }
}