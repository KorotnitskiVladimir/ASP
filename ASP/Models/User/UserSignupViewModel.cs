namespace ASP.Models.User;

public class UserSignupViewModel
{
    public UserSignupFormModel? FormModel { get; set; }
    public Dictionary<string, string>? ValidationErrors { get; set; }
}