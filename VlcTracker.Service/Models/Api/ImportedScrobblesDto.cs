namespace VlcTracker.Service.Models.Api;

public class ImportedScrobblesDto
{
    public required string FileName { get; init; }
    public required int Count { get; init; }
    public string? Title { get; init; }
    public double? VideoDuration { get; init; }
    public DateTime? Date { get; init; }
}