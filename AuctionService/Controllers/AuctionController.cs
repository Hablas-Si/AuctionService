using AuctionService.Repositories;
using Microsoft.AspNetCore.Mvc;
using AuctionService.Models;

namespace AuctionService.Controllers
{
    [ApiController]
    [Route("auctions")]
    public class BiddingController : ControllerBase
    {
        private readonly ILogger<BiddingController> _logger;
        private readonly IConfiguration _config;
        private readonly IAuctionRepository _service;

        public BiddingController(ILogger<BiddingController> logger, IConfiguration config, IAuctionRepository service)
        {
            _logger = logger;
            _config = config;
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAuction([FromBody] Auction auction)
        {
            // Submit the Auction
            await _service.CreateAuction(auction);

            // Return success response
            return Ok("Auction submitted");
        }
    }
}