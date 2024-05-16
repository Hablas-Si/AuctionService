using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using AuctionService.Repositories;
using AuctionService.Models;
using MongoDB.Bson;

namespace AuctionService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuctionController : ControllerBase
    {
        private readonly IAuctionRepository _auctionService;
        private readonly ILogger<AuctionController> _logger;

        public AuctionController(ILogger<AuctionController> logger, IAuctionRepository auctionservice)
        {
            _auctionService = auctionservice;
            _logger = logger;
        }

        [HttpGet("{auctionID}")]
        public async Task<IActionResult> GetAuction(Guid auctionID)
        {
            _logger.LogInformation("Getting auction with id {auctionID}", auctionID);
            var auction = await _auctionService.GetAuction(auctionID);

            if (auction == null)
            {
                return NotFound(); // Return 404 if auction is not found
            }

            return Ok(auction); // Return auction if found
        }
        [HttpPost]
        public async Task<IActionResult> SubmitAuction([FromBody] NewAuctionAction auctionRequest)
        {
            // Check if the request is null or invalid
            if (auctionRequest == null || !ModelState.IsValid)
            {
                return BadRequest("Invalid auction data");
            }
            try
            {
                // Fetch the item data from the other service
                Console.WriteLine(auctionRequest.Item);
                // Fetch the item data from the other service
                Item dbItem; // Use Item for deserialization

                using (var client = new HttpClient())
                {
                    // Assuming the other service is running on localhost:5213
                    var response = await client.GetAsync($"http://localhost:5213/Catalog/{auctionRequest.Item}");
                    if (!response.IsSuccessStatusCode)
                    {
                        return BadRequest("Failed to fetch item data");
                    }

                    // Read the response content as a string
                    var content = await response.Content.ReadAsStringAsync();

                    // Deserialize the content string to Item
                    dbItem = JsonSerializer.Deserialize<Item>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true // Ensure case-insensitive property matching
                    });

                    // Handle case where dbItem is null or has default values
                    if (dbItem == null || string.IsNullOrWhiteSpace(dbItem.Title) || dbItem.Price <= 0)
                    {
                        return BadRequest("Invalid item data");
                    }
                }

                // Create a new auction using the provided data
                var auction = new Auction(auctionRequest.Start, auctionRequest.End, dbItem);

                // Submit the auction
                await _auctionService.SubmitAuction(auction);

                // Return success response
                return Ok("Auction submitted");
            }
            catch (Exception ex)
            {
                // Log any errors that occur during the process
                _logger.LogError(ex, "An error occurred while submitting auction.");
                return StatusCode(500, "Internal server error");
            }
        }
            [HttpPut("{auctionID}/bid")]
            public async Task<IActionResult> UpdateHighBid(Guid auctionID, [FromBody] HighBid newHighBid)
        {
            var auction = await _auctionService.GetAuction(auctionID);
            if (auction == null)
            {
                return NotFound(); // Return 404 if auction is not found
            }

            // Update the high bid
            await _auctionService.UpdateHighBid(auctionID, newHighBid);

            return Ok("High bid updated successfully");
        }
    }


}