using System;
using System.Collections.Generic;

namespace MessageApi.Models.DTOs
{
    public class OrderReadDto
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public int Number { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? Notes { get; set; }
        public decimal Total { get; set; }
        public ICollection<OrderDetailReadDto> OrderDetails { get; set; } = new List<OrderDetailReadDto>();
    }
}