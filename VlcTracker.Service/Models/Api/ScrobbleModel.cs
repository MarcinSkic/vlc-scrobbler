using System.Diagnostics.CodeAnalysis;
using VlcTracker.Service.Persistence.Entities;

namespace VlcTracker.Service.Models.Api;

public record ScrobbleModel(
    Guid Id,
    string FileName,
    string? Title,
    bool InRepeat,
    int? Duration,
    DateTime? Date
)
{
    [return: NotNullIfNotNull(nameof(scrobble))]
    public static ScrobbleModel? FromScrobble(Scrobble? scrobble)
    {
        return scrobble is not null
            ? new ScrobbleModel(
                scrobble.Id,
                scrobble.FileName,
                scrobble.Title,
                scrobble.InRepeat,
                (int?)scrobble.VideoDuration,
                scrobble.Date
            )
            : null;
    }
}
