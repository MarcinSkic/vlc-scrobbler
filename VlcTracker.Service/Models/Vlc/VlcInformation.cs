using System.Text.Json.Serialization;

namespace VlcTracker.Service.Models.Vlc;

public class VlcInformation
{
    [JsonPropertyName("category")]
    public required VlcCategory Category { get; init; }
}
