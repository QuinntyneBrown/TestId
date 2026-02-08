using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using TestId.Cli.Commands;
using TestId.Cli.Options;
using TestId.Cli.Services;

namespace TestId.Cli;

class Program
{
    static async Task<int> Main(string[] args)
    {
        // Build configuration from the assembly's directory so it works when installed as a global tool
        var assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var configuration = new ConfigurationBuilder()
            .SetBasePath(assemblyDir)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        // Setup dependency injection
        var services = new ServiceCollection();

        // Configure logging
        services.AddLogging(builder =>
        {
            builder.AddConfiguration(configuration.GetSection("Logging"));
            builder.AddConsole();
        });

        // Configure options
        var testIdOptions = new TestIdOptions();
        configuration.GetSection(TestIdOptions.SectionName).Bind(testIdOptions);
        services.AddSingleton(testIdOptions);

        // Register services
        services.AddSingleton<GitCloneService>();
        services.AddSingleton<PythonScriptService>();
        services.AddSingleton<ClipboardService>();
        services.AddSingleton<GenerateCommand>();

        var serviceProvider = services.BuildServiceProvider();

        // Create root command
        var rootCommand = new RootCommand("TestId CLI - Generates test IDs from cloned repositories");
        
        // Add generate command
        var generateCommand = serviceProvider.GetRequiredService<GenerateCommand>();
        rootCommand.AddCommand(generateCommand);

        return await rootCommand.InvokeAsync(args);
    }
}
