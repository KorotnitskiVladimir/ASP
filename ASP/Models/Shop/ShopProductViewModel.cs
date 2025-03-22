using ASP.Data.Entities;

namespace ASP.Models.Shop;

public class ShopProductViewModel
{
    public Product? Product { get; set; }
    public List<BreadCrumb> BreadCrumbs { get; set; } = new();
}