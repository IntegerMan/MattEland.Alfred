using LLama.Common;
using LLama;
using System.Text;
using LLama.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static LLama.LLamaTransforms;

namespace MattEland.Alfred;

public class AlfredBrain
{
    private readonly ILLamaExecutor _executor;
    private readonly ChatSession _session;
    private readonly ILogger<AlfredBrain> _log;
    private readonly IOptions<AlfredOptions> _options;
    private readonly InferenceParams _inferenceParams;

    public AlfredBrain(AlfredModelWrapper model, ILogger<AlfredBrain> log, IOptions<AlfredOptions> options) {
        _log = log;
        _options = options;
        _executor = new InteractiveExecutor(model.Model);

        _session = new ChatSession(_executor) {
            OutputTransform = new KeywordTextOutputStreamTransform(new string[] { $"{UserName}:", $"{BotName}:", "Assistant:", "User:" }, redundancyLength: 8, removeAllMatchedTokens: true)
        };

        _inferenceParams = new InferenceParams() {
            Temperature = _options.Value.Temperature,
            AntiPrompts = new List<string> { $"{UserName}:", "User:" },
        };
    }

    public string UserName => _options.Value.UserName;
    public string BotName => _options.Value.BotName;

    public string SessionDirectory => _options.Value.SessionPath;

    public bool LoadLastSession() {
        try {
            if (_options.Value.LoadSessionState) {
                _session.LoadSession(SessionDirectory);
                _log.LogDebug(message: $"Session loaded from {SessionDirectory}");
                return true;
            }
            return false;
        }
        catch (LLama.Exceptions.RuntimeError ex) {
            _log.LogError(ex, $"Could not load session information from {SessionDirectory}. The state may be from a different model.");
        }
        catch (IOException ex) {
            _log.LogError(ex, $"Could not load session information from {SessionDirectory}: {ex.Message}");
        }
        return false;
    }

    public void Initialize()
    {
        _session.History.AddMessage(AuthorRole.System, _options.Value.Prompt);
    }

    public string SayGreeting()
    {
        const string message = "Hello, Batman. How can I assist you?";
        _session.History.AddMessage(AuthorRole.Assistant, message);
        
        return message;
    }

    public string GetResponseToMessage(string message)
    {
        StringBuilder sb = new();
        foreach (string chunk in _session.Chat(message, _inferenceParams))
        {
            _log.LogDebug(chunk);
            sb.Append(chunk);
        }

        return sb.ToString();
    }

    public void SaveSession() {
        try {
            if (_options.Value.SaveSessionState) {
                _session.SaveSession(SessionDirectory);
                _log.LogDebug($"Session saved to {SessionDirectory}");
            }
        }
        catch (IOException ex) {
            _log.LogError(ex, $"Could not save session information: {ex.Message}");
        }
    }
}