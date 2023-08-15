using LLama.Common;
using Microsoft.Extensions.Logging;

namespace MattEland.Alfred;

public class AlfredLlamaLogger : ILLamaLogger {
    private readonly ILogger _log;

    public AlfredLlamaLogger(ILogger log) {
        _log = log;
    }

    public void Log(string source, string message, ILLamaLogger.LogLevel level) {
        EventId eventId = new EventId(-1, source);

        switch (level) {
            case ILLamaLogger.LogLevel.Info:
                _log.LogInformation(eventId, message);
                break;
            case ILLamaLogger.LogLevel.Debug:
                _log.LogDebug(eventId, message);
                break;
            case ILLamaLogger.LogLevel.Warning:
                _log.LogWarning(eventId, message);
                break;
            case ILLamaLogger.LogLevel.Error:
                _log.LogError(eventId, message);
                break;
        }
    }
}
