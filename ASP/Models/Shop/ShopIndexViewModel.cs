using ASP.Data.Entities;

namespace ASP.Models.Shop;

public class ShopIndexViewModel
{
    public List<Category> Categories { get; set; } = new();
}