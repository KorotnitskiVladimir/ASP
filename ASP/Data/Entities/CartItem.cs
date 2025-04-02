using System.ComponentModel.DataAnnotations.Schema;

namespace ASP.Data.Entities;

public record CartItem
{
    public Guid Id { get; set; }
    public Guid CartId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? ActionId { get; set; }
    public int Quantity { get; set; } = 1;
    [Column(TypeName = "decimal(14, 2)")]
    public double Price { get; set; }
    public Cart Cart { get; set; } = null!;
    public Product Product { get; set; } = null!;
}