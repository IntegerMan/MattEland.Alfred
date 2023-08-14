namespace MattEland.Alfred;

public interface ISpeechProvider {
    void Say(string text);
    Task SayAsync(string text);
}
