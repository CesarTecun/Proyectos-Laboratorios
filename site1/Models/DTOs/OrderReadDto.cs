namespace HelloApi.Models.DTOs
{
    public class OrderReadDto
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string PersonName { get; set; } = string.Empty;
        public int Number { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderDetailReadDto> OrderDetails { get; set; } = [];
        public decimal Total => OrderDetails.Sum(od => od.Total);
    }
}
