using VlcTracker.Service.Models.Api;

namespace VlcTracker.Service.Services;

public interface IStatusService
{
    void SetStatus(Status status);

    Status GetStatus();
}
