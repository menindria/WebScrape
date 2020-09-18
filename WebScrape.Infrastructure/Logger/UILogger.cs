using System;
using WebScrape.Application.Contracts;

namespace WebScrape.Infrastructure.Logger
{
    public class UILogger : IUILogger
    {
        public void FailedScraping(string message)
        {
            Console.WriteLine(message);
        }
    }
}
