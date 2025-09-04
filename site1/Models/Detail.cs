using System.ComponentModel.DataAnnotations.Schema;
namespace HelloApi.Models
{
    public class Detail
    {
        public int Id { get; set; }
        public int StatusId { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int InvoiceId { get; set; }
        public Invoice? Invoice { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
