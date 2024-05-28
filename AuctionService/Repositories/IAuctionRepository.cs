using AuctionService.Models;

namespace AuctionService.Repositories
{
    public interface IAuctionRepository
    {
        Task<IEnumerable<Auction>> GetAllAuctions();
        Task<Auction> GetAuction(Guid auctionID);
        Task SubmitAuction(Auction auction);

    }
}