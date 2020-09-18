namespace WebScrape.Common
{
    public interface IConfiguration
    {
        string CurrencyListUrl { get; }
        string RateUrl { get; }
        string StoragePath { get; }
    }
}