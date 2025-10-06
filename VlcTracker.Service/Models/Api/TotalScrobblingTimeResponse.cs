namespace VlcTracker.Service.Models.Api;

public class TotalScrobblingTimeResponse
{
    public required double TotalVideoDuration { get; init; }
    public required double TotalScrobbleDuration { get; init; }
}
