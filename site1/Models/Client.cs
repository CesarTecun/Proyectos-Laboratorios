namespace HelloApi.Models
{
    public class Client
    {
        public int Id { get; set; }
        public required string FirstName { get; set; } = string.Empty;
        public required string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public required string Nit { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<Invoice> Invoices { get; set; } = [];
    }
}
