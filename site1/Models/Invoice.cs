namespace HelloApi.Models
{
    public class Invoice
    {
        public int Id { get; set; }
        public required string Serial { get; set; } = string.Empty;
        public required int Number { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int ClientId { get; set; }
        public Client? Client { get; set; }
        public List<Detail> Details { get; set; } = [];
    }
}
