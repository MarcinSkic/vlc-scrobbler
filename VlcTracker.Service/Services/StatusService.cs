using VlcTracker.Service.Models.Api;

namespace VlcTracker.Service.Services;

public class StatusService : IStatusService
{
    private Status _status = new(
        Message: "Scrobbler is initializing",
        CurrentScrobble: null,
        ScrobbleDuration: null,
        TotalDuration: null
    );

    public void SetStatus(Status status)
    {
        _status = status;
    }

    public Status GetStatus()
    {
        return _status;
    }
}
