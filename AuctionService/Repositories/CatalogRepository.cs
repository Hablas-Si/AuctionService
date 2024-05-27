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

        private readonly IHttpContextAccessor _httpContextAccessor;

        public CatalogRepository(HttpClient httpClient, ILogger<CatalogRepository> logger, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        // public async Task<HttpResponseMessage> GetSpecificCatalog(Guid ItemId)
        // {
        //     var response = await _httpClient.GetAsync($"/api/catalog/{ItemId}");
        //     response.EnsureSuccessStatusCode();
        //     return response;
        // }

        public async Task<HttpResponseMessage> GetTask(Guid itemId)
        {
            var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));

            var response = await _httpClient.GetAsync($"/api/catalog/{itemId}");
            response.EnsureSuccessStatusCode();
            return response;
        }

    }
}