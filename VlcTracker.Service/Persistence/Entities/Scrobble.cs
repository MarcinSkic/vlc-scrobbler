namespace VlcTracker.Service.Persistence.Entities;

public class Scrobble
{
    public Guid Id { get; init; }
    public required string FileName { get; init; }
    public string? Title { get; init; }
    public bool InRepeat { get; init; }

    /// <summary>
    /// In seconds
    /// </summary>
    public double? VideoDuration { get; set; }

    public double? ScrobbleDuration { get; set; }

    public required DateTime? Date { get; init; }
    
    public DateTime CreatedAt { get; private init; } = DateTime.UtcNow;
}
