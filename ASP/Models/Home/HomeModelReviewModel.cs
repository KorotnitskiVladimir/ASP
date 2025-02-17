using Microsoft.AspNetCore.Mvc;

namespace ASP.Models.Home;

public class HomeModelReviewModel
{
    [FromForm(Name = "author")] public string Author { get; set; } = null!;
    [FromForm(Name = "review")] public string Review { get; set; } = null!;
    [FromForm(Name = "rate")] public int Rate { get; set; }
    [FromForm(Name = "date")] public DateOnly Date { get; set; }
}