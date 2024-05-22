﻿using AuctionService.Models;

namespace AuctionService.Repositories
{
    public interface IAuctionRepository
    {
        Task<Auction> GetAuction(Guid auctionID);
        Task SubmitAuction(Auction auction);
        Task UpdateHighBid(Guid auctionID, HighBid newHighBid);

    }
}