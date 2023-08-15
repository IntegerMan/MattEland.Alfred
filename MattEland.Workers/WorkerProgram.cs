using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MattEland.Workers;


public abstract class WorkerProgram {

    public virtual string HostConfigSection { get; } = "WorkerHost";
    public virtual string LoggingConfigSection { get; } = "Logging";
    public abstract string EnvironmentVariablePrefix { get; }

    public int Run(string[] args) {
        IHostBuilder builder =
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) => {
                    services.AddSingleton<IHost, WorkerHost>();
                    services.AddOptions<WorkerHostOptions>()
                            .Configure(options => context.Configuration.GetSection(HostConfigSection).Bind(options));
                })
                .ConfigureAppConfiguration(services => {
                    services.AddEnvironmentVariables(prefix: EnvironmentVariablePrefix);
                })
                // TODO: This might not be needed
                .ConfigureLogging((hostContext, config) => {
                    config.AddConfiguration(hostContext.Configuration.GetSection(LoggingConfigSection));
                    config.AddConsole();
                });

        builder = ConfigureHostBuilder(builder, args);

        return RunHost(builder);
    }

    protected int RunHost(IHostBuilder builder) {
        try {
            using WorkerHost host = (WorkerHost)builder.Build();
            host.Run();

            return host.Succeeded ? 0 : -1;
        }
        catch (OptionsValidationException ex) {
            // Validate the provided configuration options
            WriteErrorToConsole(ex.Message);
            WriteHelpToConsole(UsageHelp);
            return -2;
        }
        catch (AggregateException ex) {
            // This typically happens when a worker encounters an exception shutting down
            WriteErrorToConsole(ex.Message);

            return -3;
        }
    }

    protected abstract IHostBuilder ConfigureHostBuilder(IHostBuilder builder, string[] args);

    public abstract string UsageHelp { get; }

    public static void WriteErrorToConsole(string message) {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine();
        Console.Error.WriteLine(message);
        Console.WriteLine();
        Console.ResetColor();
    }

    public static void WriteHelpToConsole(string message) {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}


