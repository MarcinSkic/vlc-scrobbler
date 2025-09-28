using CsvHelper;
using VlcTracker.Service.Models.Api;
using VlcTracker.Service.Persistence.Entities;

namespace VlcTracker.Service.Services;

public interface IScrobblesService
{
    Task<IEnumerable<ScrobbleModel>> GetScrobbles();

    Task<IEnumerable<ScrobblesGrouped>> GetScrobblesByFilename();

    Task AddScrobbleAsync(Scrobble scrobble, CancellationToken cancellationToken);

    Task UpdateScrobbleAsync(Scrobble scrobble, CancellationToken cancellationToken);

    Task<int> ImportScrobblesFromCsv(CsvReader csvStream, CancellationToken cancellationToken);
}
