using VlcTracker.Service.Models.Api;

namespace VlcTracker.Service.Services;

public interface IApiService
{
    Task<IEnumerable<ScrobbleModel>> GetScrobbles();
}
