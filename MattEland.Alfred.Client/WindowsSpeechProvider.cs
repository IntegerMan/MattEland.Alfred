using System.Speech.Synthesis;

namespace MattEland.Alfred.Client;

public class WindowsSpeechProvider : IDisposable, ISpeechProvider {

    private readonly SpeechSynthesizer _speech = new();
    private readonly string _voice;

    public WindowsSpeechProvider() {
        _speech.SetOutputToDefaultAudioDevice();
        var voices = _speech.GetInstalledVoices();
        if (voices.Any(v => v.VoiceInfo.Name == "CereVoice Giles - English (England)")) {
            _speech.SelectVoice("CereVoice Giles - English (England)");
        } else {
            _speech.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Senior);
        }
        _voice = _speech.Voice.Name;
    }

    public void Say(string text) {

        // Speak a string.  
        PromptBuilder builder = ConfigureVoice(text);
        _speech.Speak(builder);
    }


    public async Task SayAsync(string text) {
        // Speak a string.  
        PromptBuilder builder = ConfigureVoice(text);
        _speech.SpeakAsync(builder);
    }

    private PromptBuilder ConfigureVoice(string text) {
        PromptBuilder builder = new();
        builder.StartVoice(_voice);
        builder.StartStyle(new PromptStyle() { Rate = PromptRate.Fast, Emphasis = PromptEmphasis.Moderate });
        builder.AppendText(text);
        builder.EndStyle();
        builder.EndVoice();

        return builder;
    }

    public void Dispose() {
        _speech?.Dispose();
    }
}
