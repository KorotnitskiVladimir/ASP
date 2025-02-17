using Microsoft.AspNetCore.Mvc;

namespace ASP.Models.Home;

public class HomeAjaxReviewModel
{
    [ModelBinder(Name = "Author")] public string Author { get; set; } = null!;
    [ModelBinder(Name = "Review")] public string Review { get; set; } = null!;
    [ModelBinder(Name = "Rate")] public int Rate { get; set; }
    [ModelBinder(Name = "Date")] public DateOnly Date { get; set; }
}