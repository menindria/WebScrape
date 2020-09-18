using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebScrape.Application.Contracts
{
    public interface ICurrencyListProvider
    {
        Task<IEnumerable<string>> Execute();
    }
}