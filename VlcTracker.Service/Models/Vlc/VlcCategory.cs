using System.Text.Json.Serialization;

namespace VlcTracker.Service.Models.Vlc;

public class VlcCategory
{
    [JsonPropertyName("meta")]
    public required VlcMeta Meta { get; init; }
}