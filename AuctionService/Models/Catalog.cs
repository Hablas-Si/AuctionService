using System;

namespace AuctionService.Models
{
    public class Catalog
    {
        public Guid Id { get; set; }
        public int? ItemId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }

        public string? ImageUrl { get; set; }
        public bool? ProductAvailable { get; set; }

        public ExtendedCatalogInfo? ExtendedCatalog { get; set; }



        // Indre klasse for ExtendedCatalog
        public class ExtendedCatalogInfo
        {
            public DateTime StartDate { get; set; }
            public DateTime SoldDate { get; set; }
            public string? AuctionAdmin { get; set; }
            public string? Seller { get; set; }
            public string? Buyer { get; set; }
        }
    }
}
