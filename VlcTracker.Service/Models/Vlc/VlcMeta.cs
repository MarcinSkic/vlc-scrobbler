using System.Text.Json.Serialization;

namespace VlcTracker.Service.Models.Vlc;

public class VlcMeta
{
    [JsonPropertyName("filename")]
    public required string FileName { get; init; }
    
    [JsonPropertyName("title")]
    public string? Title { get; init; }
}