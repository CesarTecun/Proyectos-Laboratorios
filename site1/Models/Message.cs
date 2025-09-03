using System.ComponentModel.DataAnnotations.Schema;

namespace MessageApi.Models;

public class Message
{
    public int Id { get; set; }

    [Column("Text")] 
    public string MessageText { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
