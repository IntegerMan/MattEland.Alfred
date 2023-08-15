using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MattEland.Alfred.Cli;

public class AlfredCliProgram : AlfredProgram {
    public override string UsageHelp => @"Usage: AlfredCli -m C:\Models\MyModel.bin";

    protected override IHostBuilder ConfigureHostBuilder(IHostBuilder builder, string[] args) =>
        builder.UseConsoleLifetime()
            .ConfigureServices((context, services) => {
                // Dependencies needed by our worker

                // Detect Options
                services.AddOptions<AlfredCliOptions>()
                        .BindConfiguration("AlfredCli")
                        .ValidateDataAnnotations();

                // Register our service
                services.AddHostedService<AlfredCliWorker>();
            })
            .ConfigureAppConfiguration(services => {
                services.AddCommandLine(args, new Dictionary<string, string>() {
                    { "-m", "AlfredCli:Model" },
                    { "--repository", "AlfredCli:Model" },
                });
            });
}


