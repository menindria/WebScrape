using WebScrape.Application.Contracts;

namespace WebScrape.Infrastructure.Storage
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly string _path;

        public RepositoryFactory(string path)
        {
            _path = path;
        }

        public IRepository Create()
        {
            return new CsvRepository(_path);
        }
    }
}
