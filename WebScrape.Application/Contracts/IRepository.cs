using System.Collections.Generic;
using WebScrape.Domain.Models;

namespace WebScrape.Application.Contracts
{
    public interface IRepository
    {
        void Save(IEnumerable<Rate> data, string fileName);
    }
}