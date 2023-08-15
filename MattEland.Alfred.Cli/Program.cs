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

        LLamaDefaultLogger logger = LLamaDefaultLogger.Default.EnableConsole().EnableFile("llamadebug.log");
        using AlfredBrain alfred = new(modelPath, logger);
        using WindowsSpeechProvider speech = new();
        
        AlfredClient client = new(alfred, speech);

        client.ConductConversation();
    }
}