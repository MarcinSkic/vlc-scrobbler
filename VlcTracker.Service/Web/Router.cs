using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using VlcTracker.Service.Models.Api;
using VlcTracker.Service.Services;

namespace VlcTracker.Service.Web;

public static class Router
{
    public static WebApplication MapAppEndpoints(this WebApplication app)
    {
        app.MapGet("/", (IStatusService statusService) => Results.Ok(statusService.GetStatus())).Produces<Status>();

        app.MapGet(
            "/scrobbles",
            async (IScrobblesService scrobblesService) =>
                Results.Ok(await scrobblesService.GetScrobbles())
        ).Produces<IEnumerable<ScrobbleModel>>();

        app.MapGet(
            "/scrobbles/filenames",
            async (IScrobblesService scrobblesService) =>
                Results.Ok(await scrobblesService.GetScrobblesByFilename())
        ).Produces<IEnumerable<ScrobblesGroupedResponse>>();

        app.MapGet(
            "/scrobbles/total-time",
            async (IScrobblesService scrobblesService) =>
                Results.Ok(await scrobblesService.GetTotalScrobblingTime())
        ).Produces<TotalScrobblingTimeResponse>();

        app.MapPost(
                "/scrobbles/import/csv",
                async (
                    IFormFile file,
                    IScrobblesService scrobblesService,
                    CancellationToken cancellationToken
                ) =>
                {
                    if (file.Length == 0)
                        return Results.BadRequest("No file uploaded.");

                    await using var stream = file.OpenReadStream();
                    using var reader = new StreamReader(stream);
                    using var csv = new CsvReader(
                        reader,
                        new CsvConfiguration(CultureInfo.InvariantCulture)
                        {
                            HeaderValidated = null,
                            MissingFieldFound = null,
                        }
                    );

                    var importedCount = await scrobblesService.ImportScrobblesFromCsv(
                        csv,
                        cancellationToken
                    );
                    return Results.Ok(new { Count = importedCount });
                }
            )
            .DisableAntiforgery();

        return app;
    }
}
