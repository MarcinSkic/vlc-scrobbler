namespace VlcTracker.Service.Models.Api;

public class ScrobblesGrouped
{
    public required string FileName { get; init; }
    public required double? Duration { get; init; }
    public required int RepeatCount { get; init; }
    public required int Count { get; init; }
};