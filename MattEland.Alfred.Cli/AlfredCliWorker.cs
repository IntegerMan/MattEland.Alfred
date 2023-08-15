using LLama.Common;
using MattEland.Alfred.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MattEland.Alfred.Cli;

public class AlfredCliWorker : AlfredWorkerBase {
    private readonly AlfredCliOptions options;

    private static readonly Action<ILogger, Exception?> logExecuting = LoggerMessage.Define(LogLevel.Information, new EventId(1, "RunningExtract"), "Running Extract Task");
    private static readonly Action<ILogger, string, Exception?> logWorkError = LoggerMessage.Define<string>(LogLevel.Error, new EventId(1, "ExtractError"), "Error running extract task: {ErrorMessage}");
    private static readonly Action<ILogger, string, Exception?> logCompleted = LoggerMessage.Define<string>(LogLevel.Information, new EventId(1, "Cloned"), "Repository cloned to {Path}");

    private Timer? _timer;

    public override string Name => "AlfredCli";

    public const int InitialDelayInSeconds = 1;

    public AlfredCliWorker(ILogger<AlfredCliWorker> log, IOptions<AlfredCliOptions> options) : base(log) {
        this.options = options.Value;
    }

    protected override async Task OnStartAsync() {
        _timer = new Timer(DoWork, null, TimeSpan.FromSeconds(InitialDelayInSeconds), TimeSpan.FromMilliseconds(-1));

        await base.OnStartAsync();
    }

    private void DoWork(object? state) {
        bool succeeded = true;

        try {
            string modelPath = @"C:\Models\wizardLM-7B.ggmlv3.q4_1.bin";

            LLamaDefaultLogger logger = LLamaDefaultLogger.Default.EnableConsole().EnableFile("llamadebug.log");
            using AlfredBrain alfred = new(modelPath, logger);
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


