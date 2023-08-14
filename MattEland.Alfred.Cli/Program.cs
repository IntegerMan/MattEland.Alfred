using System.Speech.Synthesis;
using LLama;
using LLama.Common;

namespace MattEland.Alfred.Cli;

internal class Program
{
    private static void Main()
    {
        string modelPath = @"C:\Models\wizardLM-7B.ggmlv3.q4_1.bin";

        AlfredBrain alfred = new(modelPath);
        using var speech = new WindowsSpeechProvider();
        alfred.SpeechProvider = speech;

        alfred.LoadLastSession();
        alfred.DoCoreLoop();
        alfred.SaveSession();
    }
}