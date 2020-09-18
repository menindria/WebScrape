using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using WebScrape.Application.Contracts;

namespace WebScrape.Infrastructure.Storage
{
    public class Repository : IRepository
    {
        private readonly string _path;

        public Repository(string path)
        {
            _path = path;
        }

        public void Save(IEnumerable<Domain.Models.Rate> data, string fileName)
        {
            using (var textWriter = new StreamWriter($"{_path}\\{fileName}.csv"))
            {
                var writer = new CsvWriter(textWriter, CultureInfo.InvariantCulture);

                writer.WriteRecords(data);
            }
        }
    }
}
