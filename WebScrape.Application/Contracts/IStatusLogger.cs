namespace WebScrape.Application.Contracts
{
    public interface IStatusLogger
    {
        void FailedScraping(string message);
    }
}