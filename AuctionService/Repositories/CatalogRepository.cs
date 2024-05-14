using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.Models;

namespace AuctionService.Repositories
{
    public class CatalogRepository : ICatalogRepository
    {
        private readonly HttpClient _httpClient;

        public CatalogRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> GetCatalogAsync(int catalogId)
        {
            var response = await _httpClient.GetAsync($"/api/catalog/{catalogId}");
            response.EnsureSuccessStatusCode();
            return response;
        }
    }

}