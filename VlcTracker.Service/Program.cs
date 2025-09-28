using Microsoft.EntityFrameworkCore;
using VlcTracker.Service;
using VlcTracker.Service.Extensions;
using VlcTracker.Service.Persistence;
using VlcTracker.Service.Services;
using VlcTracker.Service.Web;

if (!Path.Exists(Constants.ApplicationDataPath))
{
    Directory.CreateDirectory(Constants.ApplicationDataPath);
}

var builder = WebApplication.CreateBuilder(args);
builder
    .Services.LoadSettings(out var settings)
    .AddDbContext<TrackingContext>(options =>
    {
        var dbPath = settings.DbPath ?? Path.Join(Constants.ApplicationDataPath, "tracker.db");
        options.UseSqlite($"Data Source={dbPath}");
    })
    .AddScoped<IScrobblesService, ScrobblesService>()
    .AddSingleton<IStatusService, StatusService>()
    .AddHostedService<ScrobblerBackgroundService>();

builder.ConfigureLogging(settings);

builder.WebHost.UseUrls($"http://localhost:{settings.ServerPort}");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TrackingContext>();
    await dbContext.Database.MigrateAsync();
}

app.MapAppEndpoints();

app.Run();
