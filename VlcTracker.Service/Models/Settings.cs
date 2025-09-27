namespace VlcTracker.Service.Models;

public class Settings
{
    public string? DbPath { get; init; }
    public string VlcPassword { get; init; } = "password";
    public bool SaveDate { get; init; } = true;
    public string VlcPort { get; init; } = "9080";

    public string ServerPort { get; init; } = "9518";

    public LogLevel LogLevel { get; init; } = LogLevel.Warning;
}
