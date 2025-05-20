using dotnet_folder_sync;
using dotnet_folder_sync.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.CommandLine;

public class Program
{
    public static async Task Main(string[] args)
    {
        var sourceOption = new Option<string>(
            name: "--source",
            description: "Source folder path")
        { IsRequired = true };

        var targetOption = new Option<string>(
            name: "--target",
            description: "Target folder path")
        { IsRequired = true };

        var logDirOption = new Option<string>(
            name: "--logs-dir",
            description: "Folder to save logs to")
        { IsRequired = true };

        var syncInterval = new Option<int>(
            name: "--interval",
            description: "Sync interval in minutes")
        { IsRequired = true };

        var rootCommand = new RootCommand
        {
            sourceOption,
            targetOption,
            logDirOption,
            syncInterval,
        };

        rootCommand.SetHandler(async (sourceOptionValue, targetOptionValue, logsDirOptionValue, syncIntervalValue) =>
        {
            var serviceProvider = new ServiceCollection()
                .AddTransient<ISyncService, SyncService>()
                .AddLogging(builder => builder.AddSerilog(CreateSerilog(logsDirOptionValue)))
                .BuildServiceProvider();

            var syncService = serviceProvider.GetRequiredService<ISyncService>();
            await syncService.Sync(sourceOptionValue, targetOptionValue, new TimeSpan(0, syncIntervalValue, 0));
        },
            sourceOption, targetOption, logDirOption, syncInterval);
        await rootCommand.InvokeAsync(args);
    }

    private static ILogger CreateSerilog(string logsDirectory)
    {
        return new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(Path.Combine(logsDirectory, "log.txt"),
                rollingInterval: RollingInterval.Day,
                rollOnFileSizeLimit: true)
            .CreateLogger();
    }
}