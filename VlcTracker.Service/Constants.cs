using System.Text.Json;

namespace VlcTracker.Service;

public static class Constants
{
    public const string UserFriendlyName = "VLC Tracker";
    public const string TechnicalName = "VlcTracker";

    public static string ApplicationDataPath { get; } =
        Path.Join(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            TechnicalName
        );

    public static JsonSerializerOptions JsonDefaultOptions { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
}