using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using VlcTracker.Service.Models;
using VlcTracker.Service.Models.Vlc;
using VlcTracker.Service.Persistence;
using VlcTracker.Service.Persistence.Entities;

namespace VlcTracker.Service.Services;

public class ScrobblerService(Settings settings, ILogger<ScrobblerService> logger, IServiceScopeFactory scopeFactory) : BackgroundService
{
    private HttpClient GetVlcClient()
    {
        var client = new HttpClient();
        client.BaseAddress = new Uri("http://localhost:9080/requests/status.json");
        client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
        
        var authentication = System.Text.Encoding.UTF8.GetBytes($":{settings.VlcPassword}");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",Convert.ToBase64String(authentication));
        
        return client;
    }

    private const double PercentageToScrobble = 0.5;
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
            try
            {
                var response = await vlcClient.GetAsync("", cancellationToken);
                var result = (await response.Content.ReadFromJsonAsync<VlcStatus>(cancellationToken))!;

                await AnalyzeVlcStatus(result,cancellationToken);
            }
            catch (HttpRequestException e)
            {
                logger.LogInformation("VLC is disabled");
                ResetCurrentScrobble();
            }
            catch (JsonException e)
            {
                logger.LogWarning("Could not deserialize VLC status as video");
                ResetCurrentScrobble();
            }
            
            await Task.Delay(TimeSpan.FromSeconds(ServiceFrequency), cancellationToken);
        }
    }

    private async Task AnalyzeVlcStatus(VlcStatus status, CancellationToken cancellationToken)
    {
        if (_currentScrobble is null)
        {
            logger.LogInformation("New Scrobble: opened VLC");
            SetNewScrobble(status);
            return;
        }

        if (_currentScrobble.FileName != status.Information.Category.Meta.FileName)
        {
            logger.LogInformation("New Scrobble: changed file");
            SetNewScrobble(status);
            return;
        }
        
        TryCalculateTotalDuration(status);

        if (TotalDuration == 0 || status.State == "paused")
        {
            return;
        }

        if (_lastPosition > status.Position)
        {
            if (_currentScrobbleSaved)
            {
                logger.LogInformation("New Scrobble: moved back scrobbled file");
                SetNewScrobble(status);
                return;
            }
            
            _currentDuration = Math.Max(_currentDuration - (_lastPosition- status.Position)*TotalDuration - ServiceFrequency,0);
        }


        _currentDuration += ServiceFrequency;
        _lastPosition = status.Position;
        logger.LogInformation("Scrobble progress: {Duration}s/{MinimumDurationToScrobble}s", _currentDuration,PercentageToScrobble*TotalDuration);
        if (_currentDuration / TotalDuration > PercentageToScrobble && !_currentScrobbleSaved && _totalDurationQueue.Count == TotalDurationQueueCapacity)
        {
            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TrackingContext>();

            _currentScrobble.Duration = (int)TotalDuration;
            await dbContext.Scrobbles.AddAsync(_currentScrobble,cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            _currentScrobbleSaved = true;
            
            logger.LogInformation("=======Scrobbled: {CurrentScrobbleFileName}============", _currentScrobble.FileName);
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

    private void SetNewScrobble(VlcStatus status)
    {
        logger.LogInformation("Start scrobbling: {FileName}", status.Information.Category.Meta.FileName);
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
            Duration = (int)TotalDuration
        };
    }

    private void ResetCurrentScrobble()
    {
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
}
