namespace AuctionService.Models
{
    public class MongoDBSettings
    {
        public string ConnectionAuction { get; set; } = "ConnectionURI=mongodb+srv://admin:admin@auctionhouse.dfo2bcd.mongodb.net/";
        public string DatabaseName { get; set; } = "AuctionDB"; //Not a secret. hardcoded
        public string CollectionName { get; set; } = "AuctionCollection"; // Not a secret. hardcoded
    }
}