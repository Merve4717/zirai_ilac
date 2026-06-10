namespace ZiraiIlacERPWeb.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string ProductName { get; set; }

        public string? Barcode { get; set; }

        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public int? CategoryId { get; set; }
    }
}
