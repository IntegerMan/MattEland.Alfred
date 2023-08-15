namespace MattEland.Alfred.Abstractions;

public interface ISpeechProvider {
    void Say(string text);
    Task SayAsync(string text);
}
