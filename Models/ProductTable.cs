namespace cldvPOE.Models
{
    public class ProductTable
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public bool Availability { get; set; }
        public string ImageUrl { get; set; }
    }
}
