using System.Collections.Concurrent;
using System.Diagnostics;
using Infrastructure.Telemetry;
using WebClient.Models;

namespace WebClient.Services;

public interface IPingRepository
{
    IEnumerable<PingModel> GetAll();
    bool TryGetModel(Guid id, out PingModel? pingModel);
    void SaveModel(PingModel model);
    PingModel? DeleteModel(Guid id);
}

public class InMemoryPingRepository : IPingRepository
{
    readonly ConcurrentDictionary<Guid, PingModel> _pings = new ();

    public IEnumerable<PingModel> GetAll()
    {
        using var activity = TelemetryConstants.ActivitySource.StartActivity($"{GetType().Name} list");

        return _pings.Values;
    }

    public bool TryGetModel(Guid id, out PingModel? pingModel)
    {
        using var activity = TelemetryConstants.ActivitySource.StartActivity($"{GetType().Name} get");
        activity?.SetTag("ping.id", id.ToString());

        return _pings.TryGetValue(id, out pingModel);
    }

    public void SaveModel(PingModel model)
    {
        using var activity = TelemetryConstants.ActivitySource.StartActivity($"{GetType().Name} set");
        activity?.SetTag("ping.id", model.Id);
        _pings[model.Id] = model;
    }

    public PingModel? DeleteModel(Guid id)
    {
        using var activity = TelemetryConstants.ActivitySource.StartActivity($"{GetType().Name} delete");
        activity?.SetTag("ping.id", id.ToString());

        _pings.Remove(id, out var model);
        return model;
    }
}
