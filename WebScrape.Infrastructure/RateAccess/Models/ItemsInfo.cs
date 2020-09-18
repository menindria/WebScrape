using System;
using System.Collections.Generic;
using System.Text;

namespace WebScrape.Infrastructure.RateAccess.Models
{
    public class ItemsInfo
    {
        public int PageSize { get; set; }
        public int Count { get; set; }
        public int NumberOfPages => (int) Math.Ceiling((decimal) Count / PageSize);
        public bool HasItems => PageSize != 0 && Count != 0;
    }
}
