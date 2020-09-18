using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebScrape.Domain.Models;

namespace WebScrape.Application.Contracts
{
    public interface IRateProvider
    {
        Task<IEnumerable<Rate>> Execute(string currency, DateTime startDate, DateTime endDate);
    }
}