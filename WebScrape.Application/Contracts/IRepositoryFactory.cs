namespace WebScrape.Application.Contracts
{
    public interface IRepositoryFactory
    {
        IRepository Create();
    }
}