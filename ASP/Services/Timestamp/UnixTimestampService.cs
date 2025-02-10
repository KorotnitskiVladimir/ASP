namespace ASP.Services.Timestamp;

public class UnixTimestampService : ITimestampService
{
    public long Timestamp => ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();

}