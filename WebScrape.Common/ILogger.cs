using System;

namespace WebScrape.Common
{
    public interface ILogger
    {
        void LogException(Exception exception);
    }
}