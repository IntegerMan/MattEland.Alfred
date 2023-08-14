using System.Speech.Synthesis;
using LLama;
using LLama.Common;
using MattEland.Alfred.Client;

namespace MattEland.Alfred.Cli;

internal class Program
{
    private static void Main()
    {
        string modelPath = @"C:\Models\wizardLM-7B.ggmlv3.q4_1.bin";

        AlfredBrain alfred = new(modelPath);

        using var speech = new WindowsSpeechProvider();
        AlfredClient client = new(alfred, speech);

        client.ConductConversation();
    }
}