namespace MattEland.Alfred.Abstractions;

public interface IAlfredTextGenerator
{
    bool LoadLastSession();
    void Initialize();
    string SayGreeting();
    string GetResponseToMessage(string message);
    void SaveSession();
    string BotName { get; }
}