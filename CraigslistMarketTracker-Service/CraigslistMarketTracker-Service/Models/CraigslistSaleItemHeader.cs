using System;

namespace CraigslistMarketTracker_Service.Models
{
    public class CraigslistSaleItemHeader
    {
        public string PostingId { get; set; }
        public string RePostId { get; set; }
        public string Title { get; set; }
        public DateTime CreateDate { get; set; }
        public string LocationTag { get; set; }
    }
}
