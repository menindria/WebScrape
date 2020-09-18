using System.IO;
using WebScrape.Application.Contracts;

namespace WebScrape.Infrastructure.Storage
{
    public class StorageInitializer : IStorageInitializer
    {
        public void Initialize(string path)
        {
            if (Directory.Exists(path))
            {
                foreach (string file in Directory.GetFiles(path))
                {
                    File.Delete(file);
                }
            }
            else
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
