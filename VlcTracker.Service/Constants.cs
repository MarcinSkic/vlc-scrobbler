using System.Text.Json;
using System.Text.Json.Serialization;

namespace VlcTracker.Service;

public static class Constants
{
    public const string UserFriendlyName = "VLC Tracker";
    private const string TechnicalName = "VlcTracker";

    public static string ApplicationDataPath { get; } =
        Path.Join(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            TechnicalName
        );

    public static JsonSerializerOptions JsonDefaultOptions { get; } =
        new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        };
}
