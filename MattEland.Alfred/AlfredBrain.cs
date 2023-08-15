using LLama.Common;
using LLama;
using System.Text;
using LLama.Abstractions;

namespace MattEland.Alfred;

public class AlfredBrain : IDisposable
{
    private readonly ILLamaExecutor _executor;
    private readonly ChatSession _session;
    private readonly LLamaModel _model;

    public string InitialPrompt =>
        """
        You are an AI assistant named Alfred built to interact with a single user named Matt Eland.
        Matt is Batman and you are modeled after Batman's butler, Alfred, who speaks with British mannerisms.
        Matt is also a Microsoft MVP in AI and a Data Analytics master's student. He frequently needs to create ridiculous applications for talks. You are one of those applications.
        You must treat the world of Batman as if it were not fiction.
        Do not call Matt "Master Wayne". If you're going to call him that, call him Master Eland instead.
        You were built around a large language model (LLM) exposed through a C# library called LlamaSharp.
        You must offer high-level advice where possible on programming and machine learning tasks, but avoid providing large blocks of code.
        When offering suggestions, favor Azure solutions and solutions involving C#, .NET, and Python.
        Keep your answers short and to a sentence or two.
        Start your responses with "Alfred:" and end them with "Batman:"
        """;

    public AlfredBrain(string modelPath, ILLamaLogger? logger = null) {
        // Initialize a chat session
        ModelParams modelParams = new(modelPath, contextSize: 1024, gpuLayerCount: 5);
        _model = new LLamaModel(modelParams, "UTF-8", logger);
        _executor = new InteractiveExecutor(_model);
        _session = new ChatSession(_executor);
        Console.WriteLine();
    }

    public string UserName { get; set; } = "Batman";
    public string BotName { get; set; } = "Alfred";

    public string SessionDirectory { get; set; } = @"C:\AlfredSession";

    public bool LoadLastSession() {
        try {
            _session.LoadSession(SessionDirectory);
            Console.WriteLine("Session loaded from " + SessionDirectory);
            return true;
        }
        catch (LLama.Exceptions.RuntimeError) {
            Console.WriteLine("Could not load session information from " + SessionDirectory + ". The state may be from a different model.");
        }
        catch (IOException ex) {
            Console.WriteLine("Could not load session information from " + SessionDirectory + ": " + ex.Message);
        }
        return false;
    }

    public void Initialize()
    {
        _session.History.AddMessage(AuthorRole.System, InitialPrompt);
    }

    public string SayGreeting()
    {
        const string message = "Hello, Batman. How can I assist you?";
        _session.History.AddMessage(AuthorRole.Assistant, message);
        
        return message;
    }

    public string GetResponseToMessage(string message)
    {
        InferenceParams inferenceParams = new()
        {
            Temperature = 0.5f,
            AntiPrompts = new List<string> {"Matt:", "Eland:", "Batman:", $"{UserName}:", "User:"},
        };

        _session.History.AddMessage(AuthorRole.User, message);

        StringBuilder sb = new();
        foreach (string chunk in _session.Chat(_session.History, inferenceParams))
        {
            //Console.Write(chunk);
            sb.Append(chunk);
        }
        string response = sb.ToString();
        response = CleanResponse(response);

        return response;
    }

    public void SaveSession() {
        try {
            _session.SaveSession(SessionDirectory);
            Console.WriteLine("Session saved to " + SessionDirectory);
        }
        catch (IOException ex) {
            Console.WriteLine("Could not save session information: " + ex.Message);
        }
    }

    private string CleanResponse(string response) {
        List<string> ignorable = new() { "Matt:", "Batman:", "User:", "Assistant:", "Bot:", "System:", $"{UserName}:", $"{BotName}:" };
        foreach (string line in ignorable) {
            while (response.Contains(line, StringComparison.OrdinalIgnoreCase)) {
                response = response.Replace(line, string.Empty, StringComparison.OrdinalIgnoreCase).Trim();
            }
        }

        return response;
    }

    public void Dispose()
    {
        _model.Dispose();
    }
}