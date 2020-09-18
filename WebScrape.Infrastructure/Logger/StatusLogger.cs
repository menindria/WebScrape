using System;
using WebScrape.Application.Contracts;

namespace WebScrape.Infrastructure.Logger
{
    public class StatusLogger : IStatusLogger
    {
        public void FailedScraping(string message)
        {
            Console.WriteLine(message);
        }
    }
}
