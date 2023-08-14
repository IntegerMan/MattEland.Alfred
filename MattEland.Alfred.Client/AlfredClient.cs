namespace MattEland.Alfred.Client;

public class AlfredClient
{
    private readonly ISpeechProvider? _speech;
    public AlfredBrain Alfred { get; set; }
    
    public AlfredClient(AlfredBrain alfred, ISpeechProvider? speech = null)
    {
        _speech = speech;
        Alfred = alfred;
    }

    public void ConductConversation()
    {

        Alfred.Initialize();
        Alfred.LoadLastSession();
        Console.WriteLine();
        
        string greeting = Alfred.SayGreeting();
        SayBotMessage(greeting);

        string? prompt = null;
        do
        {
            if (prompt != null)
            {
                string response = Alfred.GetResponseToMessage(prompt);
                SayBotMessage(response);
            }

            Console.Write("Batman: ");
            prompt = Console.ReadLine()!;
        } while (!string.IsNullOrWhiteSpace(prompt));

        Console.WriteLine("Conversation concluded");

        Alfred.SaveSession();
    }

    private void SayBotMessage(string response)
    {
        _speech?.SayAsync(response);
        Console.WriteLine($"{Alfred.BotName}: {response}");
    }
}