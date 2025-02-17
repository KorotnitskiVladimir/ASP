using Microsoft.AspNetCore.Mvc;

namespace ASP.Models.Home;

public class HomeAjaxFormModel
{
    [ModelBinder(Name = "userName")] public string UserName { get; set; } = null!;
    [ModelBinder(Name = "userEmail")] public string UserEmail { get; set; } = null!;
}