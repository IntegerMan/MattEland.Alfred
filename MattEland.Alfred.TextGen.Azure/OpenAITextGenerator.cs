using MattEland.Alfred.Abstractions;

namespace MattEland.Alfred.TextGen.Azure;

public class OpenAITextGenerator : IAlfredTextGenerator {
    public string BotName => "ALFRED";

    public OpenAITextGenerator() {

    }

    public string GetResponseToMessage(string message) {
        throw new NotImplementedException();
    }

    public void Initialize() {
        throw new NotImplementedException();
    }

    public bool LoadLastSession() {
        throw new NotImplementedException();
    }

    public void SaveSession() {
        throw new NotImplementedException();
    }

    public string SayGreeting() {
        throw new NotImplementedException();
    }
}