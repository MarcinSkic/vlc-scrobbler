namespace VlcTracker.Service.Models.Api;

public record Status(string Message, ScrobbleModel? CurrentScrobble, double? ScrobbleDuration, double? TotalDuration);
