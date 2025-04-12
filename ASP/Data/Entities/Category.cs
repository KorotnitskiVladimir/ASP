using System.Text.Json.Serialization;

namespace ASP.Data.Entities;

public record Category
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;

    public DateTime? DeletedAt { get; set; }

    // Navigational properties - ссылки на другие Entities
    [JsonIgnore]
    public Category ParentCategory { get; set; }
    public List<Product> Products { get; set; } = new();
}