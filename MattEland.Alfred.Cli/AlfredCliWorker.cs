using LLama.Common;
using MattEland.Alfred.Client;
using MattEland.Workers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MattEland.Alfred.Cli;

public class AlfredCliWorker : WorkerBase {
    private readonly AlfredCliOptions _options;

    private static readonly Action<ILogger, string, Exception?> logWorkError = LoggerMessage.Define<string>(LogLevel.Error, new EventId(1, "Error"), "Error running task: {ErrorMessage}");
    private readonly ILLamaLogger _llamaLogger;
    private Timer? _timer;

    public override string Name => "AlfredCli";

    public const int InitialDelayInSeconds = 1;

    public AlfredCliWorker(ILogger<AlfredCliWorker> log, IOptions<AlfredCliOptions> options, ILLamaLogger llamaLogger) : base(log) {
        this._options = options.Value;
        this._llamaLogger = llamaLogger;
    }

    protected override async Task OnStartAsync() {
        _timer = new Timer(DoWork, null, TimeSpan.FromSeconds(InitialDelayInSeconds), TimeSpan.FromMilliseconds(-1));

        await base.OnStartAsync();
    }

    private void DoWork(object? state) {
        bool succeeded = true;

        try {
            string modelPath = _options.ModelPath;

            using AlfredBrain alfred = new(modelPath, _llamaLogger);
            using WindowsSpeechProvider speech = new();

            AlfredClient client = new(alfred, speech);

            client.ConductConversation();
        }
        catch (LLama.Exceptions.RuntimeError ex) {
            logWorkError(Log, ex.Message, ex);
            succeeded = false;
        }
        catch (IOException ex) {
            logWorkError(Log, ex.Message, ex);
            succeeded = false;
        }

        NotifyWorkCompleted(succeeded);
    }

    protected override async Task OnStopAsync() {
        _timer?.Change(Timeout.Infinite, 0);

        await base.OnStopAsync();
    }

    protected override void Dispose(bool disposing) {
        if (!HasDisposed) {
            if (disposing) {
                _timer?.Dispose();
            }

            _timer = null;
        }
    }
}


