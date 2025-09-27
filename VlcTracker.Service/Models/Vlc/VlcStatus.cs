using System.Text.Json.Serialization;

namespace VlcTracker.Service.Models.Vlc;

public class VlcStatus
{
    [JsonPropertyName("position")]
    public double Position { get; init; }

    [JsonPropertyName("repeat")]
    public bool Repeat { get; init; }

    // TODO [MS]: Enum
    [JsonPropertyName("state")]
    public required string State { get; init; }

    [JsonPropertyName("information")]
    public required VlcInformation Information { get; init; }
}
