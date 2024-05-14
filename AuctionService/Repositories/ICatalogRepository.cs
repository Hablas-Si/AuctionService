using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuctionService.Repositories
{
    public interface ICatalogRepository
    {
        public Task<HttpResponseMessage> GetCatalogAsync(int catalogId);

    }
}