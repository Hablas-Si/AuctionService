using AuctionService.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AuctionService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuctionController : ControllerBase
    {
        private readonly ICatalogRepository _catalogService;
        private readonly ILogger<AuctionController> _logger;

        public AuctionController(ICatalogRepository catalogService, ILogger<AuctionController> logger)
        {
            _catalogService = catalogService;
            _logger = logger;
        }

        [HttpGet("{catalogId}")]
        public async Task<IActionResult> GetCatalog(int catalogId)
        {
            try
            {
                _logger.LogInformation($"Getting catalog with ID {catalogId}");
                var catalog = await _catalogService.GetCatalogAsync(catalogId);
                if (catalog == null)
                {
                    return NotFound($"Catalog with ID {catalogId} not found.");
                }
                return Ok(catalog);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
