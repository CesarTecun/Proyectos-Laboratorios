namespace MessageApi.Models.DTOs
{
    /// <summary>
    /// DTO para la lectura de un ítem existente.
    /// </summary>
    public class ItemReadDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
