using LLama;
using LLama.Common;
using MattEland.Alfred.Client;
using MattEland.Alfred.Speech.Windows;
using MattEland.Workers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;
using MattEland.Alfred.Abstractions;
using MattEland.Alfred.Llama;
using MattEland.Alfred.Speech.Azure;

namespace MattEland.Alfred.Cli;

public class AlfredProgram : WorkerProgram {
    public override string UsageHelp => @"Usage: AlfredCli -m C:\Models\MyModel.bin";

    public override string EnvironmentVariablePrefix => "ALFRED_";

    protected override IHostBuilder ConfigureHostBuilder(IHostBuilder builder, string[] args) =>
        builder.UseConsoleLifetime()
            .ConfigureServices((context, services) => {
                // Dependencies needed by our worker
                services.AddScoped<ILLamaLogger, AlfredLlamaLogger>();
                services.AddScoped<AlfredClient>();
                services.AddScoped<AlfredModelWrapper>();
                services.AddScoped<IAlfredBrain, AlfredLlamaBrain>();

                // The speech provider code relies on the Windows OS.
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                    services.AddSingleton<ISpeechProvider, AzureAiSpeechProvider>();
                }

                // Detect Options
                services.AddOptions<AlfredLlamaOptions>()
                        .BindConfiguration("Alfred")
                        .ValidateDataAnnotations();                
                services.AddOptions<AzureAiSpeechOptions>()
                        .BindConfiguration("AzureSpeech")
                        .ValidateDataAnnotations();
                services.AddOptions<AlfredCliOptions>()
                        .BindConfiguration("AlfredCli")
                        .ValidateDataAnnotations();

                // Register our service
                services.AddHostedService<AlfredCliWorker>();
            })
            .ConfigureAppConfiguration(services =>
            {
                services.AddUserSecrets<AlfredProgram>();
                services.AddCommandLine(args, new Dictionary<string, string>() {
                    { "-m", "AlfredCli:ModelPath" },
                    { "--model", "AlfredCli:ModelPath" },
                });
            });
}


