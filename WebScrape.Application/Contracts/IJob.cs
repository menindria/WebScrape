using System;
using System.Threading.Tasks;

namespace WebScrape.Application.Contracts
{
    public interface IJob
    {
        Task Execute(DateTime startDate, DateTime endDate);
    }
}