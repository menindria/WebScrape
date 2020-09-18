namespace WebScrape.Application.Contracts
{
    public interface IRateProviderFactory
    {
        IRateProvider Create();
    }
}