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
        private readonly ILogger<CatalogRepository> _logger;

        public CatalogRepository(HttpClient httpClient, ILogger<CatalogRepository> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<HttpResponseMessage> GetSpecificCatalog(Guid ItemId)
        {
            var response = await _httpClient.GetAsync($"/api/catalog/{ItemId}");
            response.EnsureSuccessStatusCode();
            return response;
        }

        public async Task<HttpResponseMessage> GetTask(Guid itemId)
        {
           var response = await _httpClient.GetAsync($"/api/catalog/{itemId}");
            response.EnsureSuccessStatusCode();
            return response;
        }

    }
}