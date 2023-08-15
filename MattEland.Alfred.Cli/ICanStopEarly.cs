using Microsoft.Extensions.Hosting;

namespace MattEland.Alfred.Cli;

public interface ICanStopEarly : IHostedService {
    string Name { get; }

    void InvokeOnCompleted(Action<ICanStopEarly, bool> listener);
}


