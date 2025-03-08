namespace ASP.Data.Entities;

public class UserAccess
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string RoleId { get; set; } = null!;
    public string Login { get; set; } = null!;
    public string Salt { get; set; } = null!;
    public string Dk { get; set; } = null!;
    
    // Навигационные свойства - это свойства (get; set;), которые ссылаются на другие сущности (Entities).
    // EntityFramework может автоматически их заполнять через связи
    public UserData UserData { get; set; } = null!;

    public UserRole UserRole { get; set; } = null!;

    public override string ToString()
    {
        return $"UserAccess: Id({Id}), UserId({UserId}), RoleId({RoleId}), Login({Login})";
    }
}