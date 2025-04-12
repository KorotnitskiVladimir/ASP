using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ASP.Data.Entities;

public record Product
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Slug { get; set; }
    public string ImagesCsv { get; set; } = string.Empty; // Comma Separated Values
    [Column(TypeName = "decimal(12, 2)")]
    public double Price { get; set; }
    public int Stock { get; set; } = 1;
    
    public DateTime? DeletedAt { get; set; }
    [JsonIgnore]
    public Category Category { get; set; } = null!;
}