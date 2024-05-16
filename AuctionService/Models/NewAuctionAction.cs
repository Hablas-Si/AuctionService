namespace AuctionService.Models
{
    public class NewAuctionAction
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public Guid Item { get; set; }

        public NewAuctionAction() 
        { }
    }
}

/*

{
  "start": "2024-05-16T07:59:18.510Z",
  "end": "2024-05-16T07:59:18.511Z",
  "item": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}

*/