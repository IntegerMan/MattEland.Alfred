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
                services.AddScoped<AlfredBrain>();

                // The speech provider code relies on the Windows OS.
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                    services.AddSingleton<ISpeechProvider, WindowsSpeechProvider>();
                }

                // Detect Options
                services.AddOptions<AlfredOptions>()
                        .BindConfiguration("Alfred")
                        .ValidateDataAnnotations();
                services.AddOptions<AlfredCliOptions>()
                        .BindConfiguration("AlfredCli")
                        .ValidateDataAnnotations();

                // Register our service
                services.AddHostedService<AlfredCliWorker>();
            })
            .ConfigureAppConfiguration(services => {
                services.AddCommandLine(args, new Dictionary<string, string>() {
                    { "-m", "AlfredCli:ModelPath" },
                    { "--model", "AlfredCli:ModelPath" },
                });
            });
}


