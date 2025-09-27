using System.Text.Json;
using VlcTracker.Service.Models;

namespace VlcTracker.Service.Extensions;

public static class StartupExtensions
{
    public static IServiceCollection LoadSettings(
        this IServiceCollection services,
        out Settings settings
    )
    {
        settings = new Settings();
        var settingsPath = Path.Join(Constants.ApplicationDataPath, "settings.json");

        try
        {
            var settingsJson = File.ReadAllText(settingsPath);
            settings =
                JsonSerializer.Deserialize<Settings>(settingsJson, Constants.JsonDefaultOptions)
                ?? settings;
        }
        catch (FileNotFoundException e)
        {
            var settingsJson = JsonSerializer.Serialize(settings, Constants.JsonDefaultOptions);
            File.WriteAllText(settingsPath, settingsJson);
        }
        catch (JsonException e)
        {
            Console.WriteLine($"Settings file located at {settingsPath} is in the wrong format");
        }

        services.AddSingleton(settings);

        return services;
    }

    public static IHostApplicationBuilder ConfigureLogging(
        this IHostApplicationBuilder builder,
        Settings settings
    )
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.SetMinimumLevel(settings.LogLevel);

        return builder;
    }
}
