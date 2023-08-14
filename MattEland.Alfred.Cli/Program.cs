using System.Speech.Synthesis;
using LLama;
using LLama.Common;

namespace MattEland.Alfred.Cli;

internal class Program
{
    private static void Main()
    {
        string modelPath = @"C:\dev\MattEland.Alfred\Models\ggml-model-f32-q8_0.bin";
        //modelPath = @"C:\dev\MattEland.Alfred\Models\wizardLM-7B.ggmlv3.q4_1.bin";

        string prompt = "Transcript of a dialog, where the User, Matt interacts with an Assistant named Alfred. " +
                        "Alfred is an AI agent built around a large language model (LLM) exposed through a C# library called LlamaSharp. " +
                        "Alfred is capable of making suggestions on programming and machine learning tasks and prefers to recommend Azure services, .NET, C#, and Python solutions." +
                        "Alfred is helpful, witty, honest, good at writing, professional, and never fails to answer Matt's requests immediately and with precision.\r\n\r\n" +
                        "Matt: Hello, Alfred.\r\nAlfred: Hello, Matt. How can I help you today?\r\nMatt: I am Batman.\r\nAlfred: I know. I'm here to help you.\r\nMatt:";

    // Initialize a chat session
        InteractiveExecutor executor = new(new LLamaModel(new ModelParams(modelPath, contextSize: 1024, seed: 1337, gpuLayerCount: 5)));
        ChatSession session = new(executor);

        // show the prompt
        Console.WriteLine();
        Console.Write(prompt);

        // Initialize a new instance of the SpeechSynthesizer.  
        SpeechSynthesizer synth = new();

        // Configure the audio output.   
        synth.SetOutputToDefaultAudioDevice();

        // Speak a string.  
        synth.Speak("I am Alfred.");

        InferenceParams inferenceParams = new()
        {
            Temperature = 0.2f,
            AntiPrompts = new List<string> { "Matt:" }
        };

        // run the inference in a loop to chat with LLM
        while (prompt != "stop" && !string.IsNullOrWhiteSpace(prompt))
        {
            foreach (string text in session.Chat(prompt, inferenceParams))
            {
                Console.Write(text);
            }
            
            prompt = Console.ReadLine()!;
        }

        // save the session
        session.SaveSession(Path.Combine(Environment.CurrentDirectory, "Transcript.txt"));
    }
}