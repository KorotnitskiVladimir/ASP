using Microsoft.AspNetCore.Mvc;

namespace ASP.Models.Admin;

public class ProductFormModel
{
    [FromForm(Name = "category-id")]
    public Guid CategoryId { get; set; }
    
    [FromForm(Name = "product-name")]
    public string Name {get; set;} = null!;

    [FromForm(Name = "product-description")]
    public string? Description {get; set;} = null!;
    
    [FromForm(Name = "product-slug")]
    public string? Slug {get; set;} = null!;

    [FromForm(Name = "product-price")] 
    public string Price { get; set; } = string.Empty;
    
    [FromForm(Name = "product-stock")]
    public int Stock { get; set; }

    [FromForm(Name ="product-image")]
    public IFormFile[] Images {get; set;} = null!;
    

}