using System;

namespace CraigslistMarketTracker_Service.Models
{
    public class CraigslistCategory
    {
        public string CatId { get; set; }
        public string Name { get; set; }
        public DateTime LastScrapeDate { get; set; }
    }
}
