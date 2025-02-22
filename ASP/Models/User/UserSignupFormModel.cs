namespace ASP.Models.User;

public class UserSignupFormModel
{
    public string UserName { get; set; } = null!;
    public string UserEmail { get; set; } = null!;
    public string UserPhone { get; set; } = null!;
    public string UserLogin { get; set; } = null!;
    public string UserPassword { get; set; } = null!;
    public string UserRepeat { get; set; } = null!;
    public DateTime BirthDate { get; set; }
    public int? TorsoSize { get; set; }
    public float? FootSize { get; set; }
    
    public string? Social { get; set; }
}