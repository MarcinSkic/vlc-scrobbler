namespace VlcTracker.Service.Models.Api;

public class ScrobblesGroupedResponse
{
    public required string FileName { get; init; }
    public required double? Duration { get; init; }
    public required double TotalScrobbleDuration { get; init; }
    public required int RepeatCount { get; init; }
    public required int Count { get; init; }
};
