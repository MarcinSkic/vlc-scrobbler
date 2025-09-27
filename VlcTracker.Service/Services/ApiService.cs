using Microsoft.EntityFrameworkCore;
using VlcTracker.Service.Models.Api;
using VlcTracker.Service.Persistence;

namespace VlcTracker.Service.Services;

public class ApiService(TrackingContext dbContext) : IApiService
{
    public async Task<IEnumerable<ScrobbleModel>> GetScrobbles()
    {
        return await dbContext
            .Scrobbles.Select(scrobble => ScrobbleModel.FromScrobble(scrobble))
            .ToListAsync();
    }
}
