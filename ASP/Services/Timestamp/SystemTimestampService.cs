namespace ASP.Services.Timestamp;

public class SystemTimestampService : ITimestampService
{
    public long Timestamp => DateTime.Now.Ticks;
}