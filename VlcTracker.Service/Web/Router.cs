using VlcTracker.Service.Services;

namespace VlcTracker.Service.Web;

public static class Router
{
    public static WebApplication MapAppEndpoints(this WebApplication app)
    {
        app.MapGet("/", (IStatusService statusService) => Results.Ok(statusService.GetStatus()));

        app.MapGet(
            "/scrobbles",
            async (IScrobblesService scrobblesService) => Results.Ok(await scrobblesService.GetScrobbles())
        );

        app.MapGet(
            "/scrobbles/filenames",
            async (IScrobblesService scrobblesService) =>
                Results.Ok(await scrobblesService.GetScrobblesByFilename())
        );

        return app;
    }
}
