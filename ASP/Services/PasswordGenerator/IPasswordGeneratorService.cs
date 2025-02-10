namespace ASP.Services.PasswordGenerator;

public interface IPasswordGeneratorService
{
    string GeneratePassword(int l);
}