namespace CarvedRock.Api.Models
{
    public class LocationInventory
    {
        public int ProductId { get; set; }
        public int LocationId { get; set; }
        public int OnHand { get; set; }
        public int OnOrder { get; set; }
    }

    public class Promotion
    {
        public int ProductId { get; set; }
        public string Description { get; set; } = null!;
        public double Discount { get; set; }
    }
}
