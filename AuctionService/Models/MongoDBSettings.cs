namespace AuctionService.Models
{
    public class MongoDBSettings
    {
        public string ConnectionURI { get; set; } = null!;
        public string DatabaseName { get; set; } = "AuctionDB"; //Not a secret. hardcoded
        public string CollectionName { get; set; } = "AuctionCollection"; // Not a secret. hardcoded
    }
}