using LLama.Common;
using MattEland.Workers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MattEland.Alfred.Cli;

public class AlfredProgram : WorkerProgram {
    public override string UsageHelp => @"Usage: AlfredCli -m C:\Models\MyModel.bin";

    public override string EnvironmentVariablePrefix => "ALFRED_";

    protected override IHostBuilder ConfigureHostBuilder(IHostBuilder builder, string[] args) =>
        builder.UseConsoleLifetime()
            .ConfigureServices((context, services) => {
                // Dependencies needed by our worker
                services.AddScoped<ILLamaLogger, AlfredLlamaLogger>();

                // Detect Options
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


