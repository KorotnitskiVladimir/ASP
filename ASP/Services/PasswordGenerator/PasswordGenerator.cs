namespace ASP.Services.PasswordGenerator;

public class PasswordGenerator : IPasswordGeneratorService
{
    public string GeneratePassword(int l) 
    {
        Random r = new Random();
        string res = "";
        for (int i = 0; i < l; i++)
        {
            int v = r.Next(9);
            res += v.ToString();
        }
        return res;
    }
}
