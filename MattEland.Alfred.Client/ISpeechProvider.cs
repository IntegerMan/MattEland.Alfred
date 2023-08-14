namespace MattEland.Alfred.Client;

public interface ISpeechProvider {
    void Say(string text);
    Task SayAsync(string text);
}
