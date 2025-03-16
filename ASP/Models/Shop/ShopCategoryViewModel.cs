using ASP.Data;
using ASP.Data.Entities;

namespace ASP.Models.Shop;

public class ShopCategoryViewModel
{
    public Category? Category { get; set; }

    public List<Category> Categories { get; set; } = new();
}