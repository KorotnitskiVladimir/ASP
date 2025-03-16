using ASP.Data.Entities;

namespace ASP.Models.Admin;

public class AdminIndexViewModel
{
    public List<Category> Categories { get; set; } = new();
}