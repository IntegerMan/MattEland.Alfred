using MattEland.Alfred.Abstractions;
using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MattEland.Alfred.Speech.Azure
{
    public class AzureAiSpeechProvider : ISpeechProvider, IDisposable 
    {
        private readonly SpeechSynthesizer _speech;
        private readonly ILogger<AzureAiSpeechProvider> _log;

        public AzureAiSpeechProvider(IOptions<AzureAiSpeechOptions> options, ILogger<AzureAiSpeechProvider> log)
        {
            _log = log;
            SpeechConfig config = SpeechConfig.FromSubscription(options.Value.SubscriptionKey, options.Value.Region);
            config.SpeechSynthesisVoiceName = options.Value.VoiceName;
            log.LogInformation("Using voice " + config.SpeechSynthesisVoiceName);
            _speech = new SpeechSynthesizer(config);
        }
        
        public void Say(string text)
        {
            SayAsync(text).Wait();
        }

        public async Task SayAsync(string text)
        {
            SpeechSynthesisResult result = await _speech.SpeakTextAsync(text);
            switch (result.Reason)
            {
                case ResultReason.Canceled:
                    _log.LogWarning("Speech synthesis cancelled");
                    break;
                case ResultReason.SynthesizingAudioCompleted:
                    _log.LogInformation("Done speaking");
                    break;
            }
        }

        public void Dispose()
        {
            _speech.Dispose();
        }
    }
}