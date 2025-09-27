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
    public int Duration { get; set; }

    public required DateTime? Date { get; init; }
}
