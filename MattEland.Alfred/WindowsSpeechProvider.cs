using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace MattEland.Alfred;

public class WindowsSpeechProvider : IDisposable, ISpeechProvider {

    private readonly SpeechSynthesizer _speech = new();

    public WindowsSpeechProvider() {
        _speech.SetOutputToDefaultAudioDevice();
        var voices = _speech.GetInstalledVoices();
        _speech.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Senior);
    }

    public void Say(string text) {

        // Speak a string.  
        PromptBuilder builder = new();
        builder.StartStyle(new PromptStyle() { Rate = PromptRate.Fast });
        builder.AppendText(text);
        builder.EndStyle();
        _speech.Speak(builder);
    }


    public async Task SayAsync(string text) {

        // Speak a string.  
        PromptBuilder builder = new();
        builder.StartStyle(new PromptStyle() { Rate = PromptRate.Fast });
        builder.AppendText(text);
        builder.EndStyle();
        _speech.SpeakAsync(builder);
    }

    public void Dispose() {
        _speech?.Dispose();
    }
}
