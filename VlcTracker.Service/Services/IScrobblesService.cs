using CsvHelper;
using VlcTracker.Service.Models.Api;
using VlcTracker.Service.Models.Api.Core;
using VlcTracker.Service.Persistence.Entities;

namespace VlcTracker.Service.Services;

public interface IScrobblesService
{
    Task<IEnumerable<ScrobbleModel>> GetScrobbles(Filters filters);

    Task<IEnumerable<ScrobblesGroupedResponse>> GetScrobblesByFilename(Filters filters);

    Task<TotalScrobblingTimeResponse> GetTotalScrobblingTime(Filters filters);

    Task AddScrobbleAsync(Scrobble scrobble, CancellationToken cancellationToken);

    Task UpdateScrobbleAsync(Scrobble scrobble, CancellationToken cancellationToken);

    Task<int> ImportScrobblesFromCsv(CsvReader csvStream, CancellationToken cancellationToken);
}
