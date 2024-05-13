namespace AuctionService.Models
{
    public class Auction
    {
        public Guid AuctionID { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public Bid HighBid { get; set; } // Holds the highest current bid
        public Item Item { get; set; }
    }

    public class Bid
    {
        public Guid UserID { get; set; }
        public int Amount { get; set; }
    }

    public class Item
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
