using Microsoft.EntityFrameworkCore;
using VlcTracker.Service;
using VlcTracker.Service.Extensions;
using VlcTracker.Service.Persistence;
using VlcTracker.Service.Services;

if (!Path.Exists(Constants.ApplicationDataPath))
{
    Directory.CreateDirectory(Constants.ApplicationDataPath);
}

var builder = Host.CreateApplicationBuilder(args);
builder.Services
    .LoadSettings(out var settings)
    .AddDbContext<TrackingContext>(options =>
    {
        var dbPath = settings.DbPath ?? Path.Join(Constants.ApplicationDataPath, "tracker.db");
        options.UseSqlite($"Data Source={dbPath}");
    }).
    AddHostedService<ScrobblerService>();

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TrackingContext>();
    await dbContext.Database.MigrateAsync();
}
host.Run();
