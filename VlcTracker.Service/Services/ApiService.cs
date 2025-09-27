using Microsoft.EntityFrameworkCore;
using VlcTracker.Service.Models.Api;
using VlcTracker.Service.Persistence;

namespace VlcTracker.Service.Services;

public class ApiService(TrackingContext dbContext) : IApiService
{
    public async Task<IEnumerable<ScrobbleModel>> GetScrobbles()
    {
        return await dbContext
            .Scrobbles.OrderByDescending(scrobble => scrobble.Date)
            .Select(scrobble => ScrobbleModel.FromScrobble(scrobble))
            .ToListAsync();
    }

    public async Task<IEnumerable<ScrobblesGrouped>> GetScrobblesByFilename()
    {
        return await dbContext
            .Scrobbles.GroupBy(scrobble => scrobble.FileName)
            .Select(grouping => new ScrobblesGrouped{
                FileName = grouping.Key,
                Duration = grouping.Where(scrobble => scrobble.Duration != 0).Average(scrobble => scrobble.Duration),
                RepeatCount = grouping.Count(s => s.InRepeat),
                Count = grouping.Count()
            })
            .OrderByDescending(scrobble => scrobble.Count)
            .ToListAsync();
    }
}
