namespace ASP.Data.Entities;

public class UserData
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public DateTime RegDate { get; init; }
    public DateTime BirthDate { get; set; }
    public int? TorsoSize { get; set; }
    public float? FootSize { get; set; }
    public string? Social { get; set; }

    public override string ToString()
    {
        return $"UserData: Id ({Id}, Name ({Name}), Email: ({Email}), Phone({Phone}), Birth date({BirthDate.Date})";
    }
}