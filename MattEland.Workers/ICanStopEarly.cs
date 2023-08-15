using Microsoft.Extensions.Hosting;

namespace MattEland.Workers;

public interface ICanStopEarly : IHostedService {
    string Name { get; }

    void InvokeOnCompleted(Action<ICanStopEarly, bool> listener);
}


