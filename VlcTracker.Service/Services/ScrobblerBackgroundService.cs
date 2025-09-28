using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using VlcTracker.Service.Models;
using VlcTracker.Service.Models.Api;
using VlcTracker.Service.Models.Vlc;
using VlcTracker.Service.Persistence;
using VlcTracker.Service.Persistence.Entities;

namespace VlcTracker.Service.Services;

public class ScrobblerBackgroundService(
    ISettings settings,
    ILogger<ScrobblerBackgroundService> logger,
    IStatusService statusService,
    IServiceScopeFactory scopeFactory
) : BackgroundService
{
    private HttpClient GetVlcClient()
    {
        var client = new HttpClient();
        client.BaseAddress = new Uri($"http://localhost:{settings.VlcPort}/requests/status.json");
        client.DefaultRequestHeaders.Accept.Add(
            MediaTypeWithQualityHeaderValue.Parse("application/json")
        );

        var authentication = System.Text.Encoding.UTF8.GetBytes($":{settings.VlcPassword}");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Basic",
            Convert.ToBase64String(authentication)
        );

        return client;
    }

    private const int ServiceFrequency = 1;
    private const int TotalDurationQueueCapacity = 7;

    private Scrobble? _currentScrobble;
    private double _currentDuration;

    private double TotalDuration
    {
        get
        {
            try
            {
                return _totalDurationQueue.OrderBy(x => x).ElementAt(_totalDurationQueue.Count / 2);
            }
            catch (ArgumentOutOfRangeException)
            {
                return 0;
            }
        }
    }

    private double _lastPosition;
    private bool _currentScrobbleSaved;
    private readonly Queue<double> _totalDurationQueue = new();

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var vlcClient = GetVlcClient();

        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(ServiceFrequency), cancellationToken);

            try
            {
                var response = await vlcClient.GetAsync("", cancellationToken);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    SetStatus("Can't connect to VLC: incorrect password");
                    continue;
                }

                var result = (
                    await response.Content.ReadFromJsonAsync<VlcStatus>(cancellationToken)
                )!;

                await AnalyzeVlcStatus(result, cancellationToken);
            }
            catch (HttpRequestException e)
            {
                await ResetCurrentScrobble(cancellationToken);

                SetStatus("VLC is disabled");
            }
            catch (JsonException e)
            {
                await ResetCurrentScrobble(cancellationToken);

                const string msg = "Could not deserialize VLC status as video";
                SetStatus(msg, logInfo: false);
                logger.LogWarning(msg);
            }
        }
    }

    private async Task AnalyzeVlcStatus(VlcStatus status, CancellationToken cancellationToken)
    {
        if (_currentScrobble is null)
        {
            await SetNewScrobble(status, cancellationToken);
            SetStatus("New Scrobble: opened VLC");
            return;
        }

        if (_currentScrobble.FileName != status.Information.Category.Meta.FileName)
        {
            await SetNewScrobble(status, cancellationToken);
            SetStatus("New Scrobble: changed file");
            return;
        }

        TryCalculateTotalDuration(status);

        if (TotalDuration == 0 || status.State == "paused")
        {
            SetStatus("VLC is paused");
            return;
        }

        if (_lastPosition > status.Position)
        {
            if (_currentScrobbleSaved)
            {
                await SetNewScrobble(status, cancellationToken);

                SetStatus("New Scrobble: moved back scrobbled file");
                return;
            }

            _currentDuration = Math.Max(
                _currentDuration
                    - (_lastPosition - status.Position) * TotalDuration
                    - ServiceFrequency,
                0
            );
        }

        _currentDuration += ServiceFrequency;
        _lastPosition = status.Position;
        SetStatus(
            $"Scrobble progress: {_currentDuration}s/{settings.PercentageRequiredToScrobble * TotalDuration}s"
        );
        if (
            _currentDuration / TotalDuration > settings.PercentageRequiredToScrobble
            && !_currentScrobbleSaved
            && _totalDurationQueue.Count == TotalDurationQueueCapacity
        )
        {
            using var scope = scopeFactory.CreateScope();
            var scrobblesService = scope.ServiceProvider.GetRequiredService<IScrobblesService>();

            _currentScrobble.VideoDuration = (int)TotalDuration;
            await scrobblesService.AddScrobbleAsync(_currentScrobble, cancellationToken);
            _currentScrobbleSaved = true;

            logger.LogInformation(
                "=======Scrobbled: {CurrentScrobbleFileName}============",
                _currentScrobble.FileName
            );
        }
    }

    private void TryCalculateTotalDuration(VlcStatus status)
    {
        var positionChange = status.Position - _lastPosition;
        if (positionChange == 0)
        {
            return;
        }

        var newTotalDuration = ServiceFrequency / positionChange;
        if (newTotalDuration <= 0)
        {
            return;
        }

        if (_totalDurationQueue.Count >= TotalDurationQueueCapacity)
        {
            _totalDurationQueue.Dequeue();
        }
        _totalDurationQueue.Enqueue(newTotalDuration);
    }

    private async Task SetNewScrobble(VlcStatus status, CancellationToken cancellationToken)
    {
        await TrySaveCurrentScrobbleTime(cancellationToken);
        
        logger.LogInformation(
            "Start scrobbling: {FileName}",
            status.Information.Category.Meta.FileName
        );
        var meta = status.Information.Category.Meta;

        _totalDurationQueue.Clear();
        _currentDuration = 0;
        _lastPosition = status.Position;
        _currentScrobbleSaved = false;

        _currentScrobble = new Scrobble
        {
            Id = Guid.NewGuid(),
            FileName = meta.FileName,
            Title = meta.Title,
            InRepeat = status.Repeat,
            VideoDuration = (int)TotalDuration,
            Date = settings.SaveDate ? DateTime.Now : null,
        };
    }

    private async Task ResetCurrentScrobble(CancellationToken cancellationToken)
    {
        await TrySaveCurrentScrobbleTime(cancellationToken);
        
        if (_currentScrobble is null)
        {
            return;
        }

        logger.LogInformation("Scrobbling reset");
        _currentScrobble = null;
        _totalDurationQueue.Clear();
        _currentDuration = 0;
        _lastPosition = 0;
        _currentScrobbleSaved = false;
    }

    private async Task TrySaveCurrentScrobbleTime(CancellationToken cancellationToken)
    {
        if (_currentScrobble is null || !_currentScrobbleSaved)
        {
            return;
        }
        
        using var scope = scopeFactory.CreateScope();
        var scrobblesService = scope.ServiceProvider.GetRequiredService<IScrobblesService>();

        // Math.Min to safeguard against VLC freezing
        _currentScrobble.ScrobbleDuration =Math.Min(_currentDuration,TotalDuration);
        await scrobblesService.UpdateScrobbleAsync(_currentScrobble,cancellationToken);
    }

    private void SetStatus(string msg, bool logInfo = true)
    {
        statusService.SetStatus(
            new Status(
                msg,
                ScrobbleModel.FromScrobble(_currentScrobble),
                _currentDuration,
                TotalDuration
            )
        );
        if (logInfo)
        {
            logger.LogInformation("{Message}", msg);
        }
    }
}
