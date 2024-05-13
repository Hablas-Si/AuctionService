using AuctionService.Models;
namespace AuctionService.Repositories
{
    public interface IAuctionRepository
    {
        public async Task CreateAuction(Auction auction)
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