using MattEland.Alfred.Abstractions;
using Microsoft.Extensions.Logging;

namespace MattEland.Alfred.Client;

public class AlfredClient
{
    private readonly ILogger<AlfredClient> _log;
    private readonly ISpeechProvider? _speech;
    public IAlfredBrain Alfred { get; set; }
    
    public AlfredClient(IAlfredBrain alfred, ILogger<AlfredClient> log, ISpeechProvider? speech = null)
    {
        _speech = speech;
        Alfred = alfred;
        _log = log;
    }

    public void ConductConversation()
    {
        Alfred.Initialize();
        Alfred.LoadLastSession();
        Console.WriteLine();
        
        string greeting = Alfred.SayGreeting();
        SayBotMessage(greeting);

        string? prompt = null;
        do {
            if (prompt != null) {
                _log.LogInformation($"Sending message: {prompt}");
                string response = Alfred.GetResponseToMessage(prompt);
                SayBotMessage(response);
                Console.WriteLine();
            }

            prompt = GetTextFromUser();
        } while (!string.IsNullOrWhiteSpace(prompt));

        _log.LogInformation("Conversation terminated");

        Alfred.SaveSession();
    }

    private static string GetTextFromUser() {
        string? prompt;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("Batman: ");
        prompt = Console.ReadLine()!;
        Console.ForegroundColor = ConsoleColor.White;
        return prompt;
    }

    private void SayBotMessage(string response)
    {
        _speech?.SayAsync(response);
        _log.LogInformation($"{Alfred.BotName}: {response}");
    }
}