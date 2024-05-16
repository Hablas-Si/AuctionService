namespace AuctionService.Models
{
    public class NewAuctionAction
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public Guid Item { get; set; }

        public NewAuctionAction() 
        { }
    }
}
