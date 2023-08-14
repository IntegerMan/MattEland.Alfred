using System.Speech.Synthesis;
using LLama;
using LLama.Common;

namespace MattEland.Alfred.Cli;

internal class Program
{
    private static void Main()
    {
        string modelPath = @"C:\Models\ggml-model-f32-q4_1.bin";

        AlfredBrain alfred = new(modelPath);
        using var speech = new WindowsSpeechProvider();
        alfred.SpeechProvider = speech;

        alfred.LoadLastSession();
        Console.WriteLine();
        alfred.DoCoreLoop();
        alfred.SaveSession();
    }
}