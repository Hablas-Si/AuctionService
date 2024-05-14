// using AuctionService.Models;
// using Microsoft.Extensions.Options;
// using MongoDB.Driver;

// namespace AuctionService.Repositories
// {
//     public class AuctionRepositiory : IAuctionRepository
//     {
//         private readonly HttpClient _httpClient;

//         public AuctionRepositiory(IHttpClientFactory httpClientFactory)
//         {
//             _httpClient = httpClientFactory.CreateClient("AuctionHouseClient");
//         }

//         public async Task<HttpResponseMessage> GetAuctionDetailsAsync(int auctionId)
//         {
//             var response = await _httpClient.GetAsync($"/api/auction/{auctionId}");
//             response.EnsureSuccessStatusCode(); // Smid en undtagelse hvis statuskoden ikke er succes
//             return response;
//         }


//         public Task<HttpResponseMessage> GetAuctionDetails(int auctionId)
//         {
//             throw new NotImplementedException();
//         }


//         public async Task CreateAuction()
//         {
//             //To be implemented
//             //This gets called from catalogservice as soon as an item is created
//         }
//         public async Task UpdateMaxBid()
//         {
//             //To be implemented
//             //This is called by BiddingService as soon as a new max bid is found
//         }
//         public async Task DeleteAuction()
//         {
//             //To be implemented
//             //Only to be used in absolute emergencies. Auctions should be updated to reflect changes
//         }
//         public async Task GetAuctions()
//         {
//             //To be implemented
//             //Gets a list of all auctions
//         }

//     }
// }
