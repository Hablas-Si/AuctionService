using System.Threading.Tasks;
using AuctionService.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.Extensions.Options;
using AuctionService.Repositories;
using System.Net.Http;
using System.Security.Cryptography;
using AuctionService.Services;
using MongoDB.Bson.IO;
using System.Text.Json;

namespace AuctionService.Repositories
{
    public class AuctionRepository : IAuctionRepository
    {
        private readonly IMongoCollection<Auction> AuctionCollection;
        private RabbitMQSubscriber _subscriber;

        public AuctionRepository(IOptions<MongoDBSettings> mongoDBSettings)
        {
            // trækker connection string og database navn og collectionname fra program.cs aka fra terminalen ved export. Dette er en constructor injection.
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionAuctionDB);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            AuctionCollection = database.GetCollection<Auction>(mongoDBSettings.Value.CollectionName);
        }

        public async Task<IEnumerable<Auction>> GetAllAuctions()
        {
            // Create an empty filter to match all documents
            var filter = new BsonDocument();

            // Execute the query and return all matching documents
            return await AuctionCollection.Find(filter).ToListAsync();
        }

        public async Task<Auction> GetAuction(Guid auctionID)
        {
            Console.WriteLine("GETAUCTION REPO ENTERED");
            // Create a filter to match documents with the specified auction ID
            var filter = Builders<Auction>.Filter.Eq(a => a.Id, auctionID);

            // Execute the query and return the first matching document
            return await AuctionCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task SubmitAuction(Auction auction)
        {
            Console.WriteLine($"Bid submitted: {auction}");
            await AuctionCollection.InsertOneAsync(auction);
        }
        public async Task UpdateHighBid(Guid auctionID, HighBid newHighBid)
        {
                _subscriber = new RabbitMQSubscriber("BidToAuc");
                await _subscriber.StartListening(OnBidReceived);
        }

        //RabbitMQListener

        private async Task OnBidReceived(string message)
        {
            try
            {
                // Deserialize message to get auctionID and newHighBid
                var auctionID = ExtractAuctionID(message);
                var newHighBid = ExtractNewHighBid(message);

                // Update only the HighBid property of the auction
                var filter = Builders<Auction>.Filter.Eq(a => a.Id, auctionID);
                var update = Builders<Auction>.Update.Set(a => a.HighBid, newHighBid);
                await AuctionCollection.UpdateOneAsync(filter, update);
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately
                Console.WriteLine($"Error processing message: {ex.Message}");
            }
        }

        private object ExtractNewHighBid(string message)
        {
            // Deserialize the JSON message to a dynamic object or a specific class
            var jsonObject = JsonSerializer.Deserialize<dynamic>(message);

            // Extract the high bid from the deserialized object
            HighBid highBid = new HighBid
            {
                userName = jsonObject.userName,
                Amount = jsonObject.Amount
            };

            return highBid;
        }

        private object ExtractAuctionID(string message)
        {
            // Deserialize the JSON message to a dynamic object or a specific class
            var jsonObject = JsonSerializer.Deserialize<dynamic>(message);

            // Extract the auction ID from the deserialized object
            Guid auctionID = jsonObject.AuctionId;

            return auctionID;
        }
    }
}