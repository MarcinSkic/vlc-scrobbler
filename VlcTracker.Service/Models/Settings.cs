namespace VlcTracker.Service.Models;

public class Settings
{
    public string? DbPath { get; init; }
    public string VlcPassword { get; init; } = "password";
}