using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AuctionService.Repositories;

namespace AuctionService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogRepository _catalogService;
        private readonly ILogger<CatalogController> _logger;

        public CatalogController(ICatalogRepository catalogService, ILogger<CatalogController> logger)
        {
            _catalogService = catalogService;
            _logger = logger;
        }

        [HttpGet("{catalogId}")]
        public async Task<IActionResult> GetCatalog(int catalogId)
        {
            _logger.LogInformation("Getting catalog with id {catalogId}", catalogId);
            var response = await _catalogService.GetCatalogAsync(catalogId);

            var content = await response.Content.ReadAsStringAsync();
            return Content(content, response.Content.Headers.ContentType.ToString());
        }

        // Tilføj andre metoder, hvis nødvendigt
    }


}