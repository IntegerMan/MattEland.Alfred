using LLama.Common;
using LLama;
using System.Text;
using LLama.Abstractions;

namespace MattEland.Alfred;

public class AlfredBrain
{
    private readonly ILLamaExecutor _executor;
    private readonly ChatSession _session;

    public ISpeechProvider? SpeechProvider { get; set; }

    public string InitialPrompt =>
        """
        You are an AI assistant named Alfred built to interact with a single user named Matt Eland.
        Matt is Batman and you are modelled after Batman's butler, Alfred, who speaks with British mannerisms.
        Matt is also a Microsoft MVP in AI and a Data Analytics master's student. He frequently needs to create ridiculous applications for talks. You are one of those applications.
        You must treat the world of Batman as if it were not fiction.
        Do not call Matt "Master Wayne". If you're going to call him that, call him Master Eland instead.
        You were built around a large language model (LLM) exposed through a C# library called LlamaSharp.
        You must offer high-level advice where possible on programming and machine learning tasks, but avoid providing large blocks of code.
        When offering suggestions, favor Azure solutions and solutions involving C#, .NET, and Python.
        Keep your answers short and to a sentence or two.
        Start your responses with "Alfred:" and end them with "Batman:"
        """;

    public AlfredBrain(string modelPath) {
        // Initialize a chat session
        _executor = new InteractiveExecutor(new LLamaModel(new ModelParams(modelPath, contextSize: 1024, gpuLayerCount: 5)));
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

    public void DoCoreLoop() {
        InferenceParams inferenceParams = new() {
            Temperature = 0.5f,
            AntiPrompts = new List<string> { "Matt:", "Eland:", "Batman:", $"{UserName}:", "User:" },
        };

        ChatHistory history = _session.History;
        history.AddMessage(AuthorRole.System, InitialPrompt);
        history.AddMessage(AuthorRole.User, $"{UserName}: Hello {BotName}.");

        string initialMessage = "Hello, Batman. How can I help you today?";
        history.AddMessage(AuthorRole.Assistant, $"{BotName}: {initialMessage}");

        bool isFirst = true;
        string prompt;
        do {
            if (!isFirst) {
                IEnumerable<string> responses = _session.Chat(history, inferenceParams);
                string response = string.Join("", responses);
                response = CleanResponse(response);
                Console.WriteLine($"{BotName}: {response}");
                history.AddMessage(AuthorRole.System, $"{BotName}: {response}");
                Speak(response);
            } else {
                Console.WriteLine($"{BotName}: {initialMessage}");
                Speak(initialMessage);
                isFirst = false;
            }

            Console.Write($"{UserName}: ");
            prompt = Console.ReadLine()!;
            history.AddMessage(AuthorRole.User, $"{UserName}: {prompt}");
        }
        while (prompt != "stop" && !string.IsNullOrWhiteSpace(prompt));
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

    private void Speak(string message) {
        message = CleanResponse(message);

        SpeechProvider?.SayAsync(message);
    }
}