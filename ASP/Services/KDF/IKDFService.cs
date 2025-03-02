namespace ASP.Services.KDF;
// Key Derivation Function Service by https://datatracker.ietf.org/doc/html/rfc2898#section-5.2
public interface IKDFService
{
    string DerivedKey(string password, string salt);
}