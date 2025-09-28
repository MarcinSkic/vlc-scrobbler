namespace VlcTracker.Service.Models.Api;

public class TotalScrobblingTimeResponse
{
    public required double TotalScrobbleDurationIncludingImported { get; init; }
    public double TotalScrobbleDuration { get; init; }
}