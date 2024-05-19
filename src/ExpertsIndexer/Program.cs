using ExpertsIndexer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureHostConfiguration(configHost =>
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        configHost.SetBasePath(currentDirectory);
        configHost.AddJsonFile("hostsettings.json", optional: false);
        configHost.AddCommandLine(args);
    })
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;

        services.AddApplicationOptions(configuration);
        services.AddMicrosoftGraphForApplication(configuration);
        services.AddKernelMemory(configuration);

        services.AddLogging(configure => configure.AddConsole());

        services.AddHostedService<ExpertsIndexerHostedService>();
    });
