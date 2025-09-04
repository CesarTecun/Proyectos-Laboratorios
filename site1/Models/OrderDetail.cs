using System.ComponentModel.DataAnnotations.Schema;
namespace HelloApi.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public required int OrderId { get; set; }
        public Order Order { get; set; }
        public required int ItemId { get; set; }
        public Item Item { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public required decimal Price { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public required decimal Total { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
