using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.Models;

namespace AuctionService.Repositories
{
    public interface ICatalogRepository
    {
        Task<HttpResponseMessage> GetTask(Guid ItemId);
        Task<HttpResponseMessage> GetSpecificCatalog(Guid ItemId);
    }
}