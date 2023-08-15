using MattEland.Alfred.Client;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Speech.Synthesis;

namespace MattEland.Alfred.Speech.Windows;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "Accepted Windows Only Feature")]
public class WindowsSpeechProvider : IDisposable, ISpeechProvider {

    private readonly SpeechSynthesizer _speech = new();
    private readonly string _voice;

    public WindowsSpeechProvider(ILogger<WindowsSpeechProvider> log) {
        ReadOnlyCollection<InstalledVoice> voices = _speech.GetInstalledVoices();

        if (voices.Any(v => v.VoiceInfo.Name == "CereVoice Giles - English (England)")) {
            _speech.SelectVoice("CereVoice Giles - English (England)");
        } else {
            _speech.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Senior);
        }

        _voice = _speech.Voice.Name;
        log.LogInformation($"Selecting voice {_voice}");

        _speech.SetOutputToDefaultAudioDevice();
    }

    public void Say(string text) {
        PromptBuilder builder = ConfigureVoice(text);
        _speech.Speak(builder);
    }

    public Task SayAsync(string text) {
        PromptBuilder builder = ConfigureVoice(text);
        _ = _speech.SpeakAsync(builder);

        return Task.CompletedTask;
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
