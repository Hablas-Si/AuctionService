﻿using System.Threading.Tasks;
using AuctionService.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.Extensions.Options;
using AuctionService.Repositories;
using System.Net.Http;
using System.Security.Cryptography;

namespace AuctionService.Repositories
{
    public class AuctionRepository : IAuctionRepository
    {
        private readonly IMongoCollection<Auction> AuctionCollection;

        public AuctionRepository(IOptions<MongoDBSettings> mongoDBSettings)
        {
            // trækker connection string og database navn og collectionname fra program.cs aka fra terminalen ved export. Dette er en constructor injection.
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionAuctionDB);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            AuctionCollection = database.GetCollection<Auction>(mongoDBSettings.Value.CollectionName);
        }

        public async Task<IEnumerable<Auction>> GetAllAuctions()
        {
            // Hent alle Catalog-objekter
            var auctions = await AuctionCollection.Find(_ => true).ToListAsync();

            // Opret en liste til at gemme Catalog-objekter med tilhørende ExtendedCatalog-objekter
            var auctionList = new List<Auction>();

            // Gennemgå hvert Catalog-objekt og hent tilhørende ExtendedCatalog-objekt baseret på CatalogId
            foreach (var auction in auctionList)
            {
                // Tilføj catalog til den nye liste
                auctionList.Add(auction);
            }

            return auctionList;
        }

        public async Task<Auction> GetAuction(Guid auctionID)
        {
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
            // Update only the HighBid property of the auction
            var filter = Builders<Auction>.Filter.Eq(a => a.Id, auctionID);
            var update = Builders<Auction>.Update.Set(a => a.HighBid, newHighBid);
            await AuctionCollection.UpdateOneAsync(filter, update);
        }
    }
}