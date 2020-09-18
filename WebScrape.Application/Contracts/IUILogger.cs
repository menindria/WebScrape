namespace WebScrape.Application.Contracts
{
    public interface IUILogger
    {
        void FailedScraping(string message);
    }
}