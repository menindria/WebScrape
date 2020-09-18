using System;
using WebScrape.Application.Contracts;

namespace WebScrape.Infrastructure.Logger
{
    public class UILogger : IUILogger
    {
        public void Message(string message)
        {
            Console.WriteLine(message);
        }
    }
}
