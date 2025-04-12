namespace ASP.Data.Entities;

public class AccessToken
{
    public Guid Jti { get; set; }
    public Guid Sub { get; set; } // UserAccessId
    public Guid Aud { get; set; } // UserId
    public DateTime Iat { get; set; } = DateTime.Now;
    public DateTime? Nbf { get; set; } // Not before
    public DateTime Exp { get; set; }
    public string? Iss { get; set; } // Issuer
}