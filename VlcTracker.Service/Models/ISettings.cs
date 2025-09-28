namespace VlcTracker.Service.Models;

public interface ISettings
{
    public string? DbPath { get; }
    public double PercentageRequiredToScrobble { get;}
    public string VlcPassword { get; }
    public bool SaveDate { get; }
    public string VlcPort { get; }
    public string ServerPort { get; }
    public LogLevel LogLevel { get; }
}