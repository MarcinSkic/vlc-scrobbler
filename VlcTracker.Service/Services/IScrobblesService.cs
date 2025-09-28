using VlcTracker.Service.Models.Api;
using VlcTracker.Service.Persistence.Entities;

namespace VlcTracker.Service.Services;

public interface IScrobblesService
{
    Task<IEnumerable<ScrobbleModel>> GetScrobbles();

    Task<IEnumerable<ScrobblesGrouped>> GetScrobblesByFilename();

    Task SaveScrobble(Scrobble scrobble, CancellationToken cancellationToken);
}
