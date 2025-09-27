using VlcTracker.Service.Services;

namespace VlcTracker.Service.Web;

public static class Router
{
    public static WebApplication MapAppEndpoints(this WebApplication app)
    {
        app.MapGet("/", (IStatusService statusService) => Results.Ok(statusService.GetStatus()));
        
        app.MapGet("/scrobbles/raw", async (IApiService apiService) => Results.Ok(await apiService.GetScrobbles()));

        return app;
    }
}
