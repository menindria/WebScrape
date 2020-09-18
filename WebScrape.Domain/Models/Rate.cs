namespace WebScrape.Domain.Models
{
    public class Rate
    {
        public string CurrencyName { get; set; }
        public string BuyingRate { get; set; }
        public string CashBuyingRate { get; set; }
        public string SellingRate { get; set; }
        public string CashSellingRate { get; set; }
        public string MiddleRate { get; set; }
        public string PubTime { get; set; }
    }
}
