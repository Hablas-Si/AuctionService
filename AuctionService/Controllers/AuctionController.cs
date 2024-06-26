﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using AuctionService.Repositories;
using AuctionService.Models;
using MongoDB.Bson;
using Microsoft.AspNetCore.Authorization;

namespace AuctionService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuctionController : ControllerBase
    {

        private readonly IAuctionRepository _auctionService;
        private readonly ICatalogRepository _catalogService;
        private readonly ILogger<AuctionController> _logger;
        private readonly IVaultRepository _vaultService;


        public AuctionController(ILogger<AuctionController> logger, IAuctionRepository auctionservice, ICatalogRepository catalogService, IVaultRepository vaultService)
        {
            _auctionService = auctionservice;
            _catalogService = catalogService;
            _logger = logger;
            _vaultService = vaultService;
        }

        [HttpGet("all"), Authorize(Roles="Admin, User")]
        public async Task<IActionResult> getAll()
        {
            _logger.LogInformation("Fetching all items from auctions");
            var auctionList = await _auctionService.GetAllAuctions();

            if (auctionList == null || !auctionList.Any())
            {
                // Return a 404 error indicating no content found
                return NotFound("No items found in the database.");
            }

            return Ok(auctionList);
        }


        
        [HttpGet("{auctionID}"), Authorize(Roles="Admin")]
        public async Task<IActionResult> GetAuction(Guid auctionID)
        {
            Console.WriteLine("GETAUCTION ENTERED");
            Console.WriteLine(auctionID);
            _logger.LogInformation("Getting auction with id {auctionID}", auctionID);
            var auction = await _auctionService.GetAuction(auctionID);

            Console.WriteLine("AUCTIONSERVICE RETURNED VALUE");

            if (auction == null)
            {
                Console.WriteLine("AUCTION NOT FOUND");
                return NotFound(); // Return 404 if auction is not found
            }

            Console.WriteLine("AUCTION FOUND");
            return Ok(auction); // Return auction if found
        }


        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAuction([FromBody] NewAuctionAction auctionRequest)
        {
            if (auctionRequest == null || !ModelState.IsValid)
            {
                return BadRequest("Invalid auction data");
            }

            try
            {
                _logger.LogInformation("Received auction request: {auctionRequest}", auctionRequest);

                // Hent katalogdata fra CatalogService
                var catalogResponse = await _catalogService.GetTask(auctionRequest.Item);
                if (!catalogResponse.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to fetch item data from CatalogService. Status code: {statusCode}", catalogResponse.StatusCode);
                    return BadRequest("Failed to fetch item data from CatalogService");
                }

                var catalogContent = await catalogResponse.Content.ReadAsStringAsync();
                var catalogItem = JsonSerializer.Deserialize<Item>(catalogContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (catalogItem == null || string.IsNullOrWhiteSpace(catalogItem.Title) || catalogItem.Price <= 0)
                {
                    _logger.LogError("Invalid item data received from CatalogService");
                    return BadRequest("Invalid item data received from CatalogService");
                }

                // Opret en auktion med katalogdata
                var auction = new Auction(auctionRequest.Start, auctionRequest.End, catalogItem);

                // Indsend auktion til AuctionService
                await _auctionService.SubmitAuction(auction);

                _logger.LogInformation("Auction submitted successfully");

                return Ok(auction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating auction.");
                return StatusCode(500, "Internal server error");
            }
        }



        //[HttpPut("{auctionID}/bid"), Authorize(Roles = "User, Admin")]
        //public async Task<IActionResult> UpdateHighBid(Guid auctionID, [FromBody] HighBid newHighBid)
        //{
        //    var auction = await _auctionService.GetAuction(auctionID);
        //    if (auction == null)
        //    {
        //        return NotFound(); // Return 404 if auction is not found
        //    }

        //    // Update the high bid
        //    await _auctionService.UpdateHighBid(auctionID, newHighBid);

        //    return Ok("High bid updated successfully");
        //}

        // [HttpGet("test/{catalogId}")]
        // public async Task<IActionResult> GetCatalog(Guid catalogId)
        // {
        //     _logger.LogInformation("Getting catalog with id {catalogId}", catalogId);
        //     var response = await _catalogService.GetSpecificCatalog(catalogId);

        //     var content = await response.Content.ReadAsStringAsync();
        //     return Content(content, response.Content.Headers.ContentType.ToString());
        // }

        // OBS: TIlføj en Authorize attribute til metoderne nedenunder Kig ovenfor i jwt token creation.
        [HttpGet("authorized"), Authorize(Roles = "Admin")]
        public IActionResult Authorized()
        {

            // Hvis brugeren har en gyldig JWT-token og rollen "Admin", vil denne metode blive udført
            return Ok("You are authorized to access this resource.");
        }





    }


}
