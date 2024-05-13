using AuctionService.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AuctionService.Repositories
{
    public class AuctionRepositiory : IAuctionRepository 
    {
        private readonly IMongoCollection<Auction> AuctionCollection;
        public AuctionRepositiory(IOptions<MongoDBSettings> mongoDBSettings)
        {
            // trækker connection string og database navn og collectionname fra program.cs aka fra terminalen ved export. Dette er en constructor injection.
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            AuctionCollection = database.GetCollection<Auction>(mongoDBSettings.Value.CollectionName);
        }

        public async Task CreateAuction()
        {
            //To be implemented
            //This gets called from catalogservice as soon as an item is created
        }
        public async Task UpdateMaxBid()
        {
            //To be implemented
            //This is called by BiddingService as soon as a new max bid is found
        }
        public async Task DeleteAuction()
        {
            //To be implemented
            //Only to be used in absolute emergencies. Auctions should be updated to reflect changes
        }
        public async Task GetAuctions()
        {
            //To be implemented
            //Gets a list of all auctions
        }
    }
}
