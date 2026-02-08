using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TestId.Cli.Options;
using TestId.Cli.Services;

Console.WriteLine("=== TestId.Cli Demo ===");
Console.WriteLine();

// Locate the TestId.Cli appsettings.json from its assembly location
var cliAssemblyDir = Path.GetDirectoryName(typeof(ClipboardService).Assembly.Location)!;
Console.WriteLine($"CLI assembly location: {cliAssemblyDir}");
Console.WriteLine();

// Build configuration using the CLI's appsettings.json
var configuration = new ConfigurationBuilder()
    .SetBasePath(cliAssemblyDir)
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

// Display current logging configuration
Console.WriteLine("--- Logging Configuration (from appsettings.json) ---");
var loggingSection = configuration.GetSection("Logging:LogLevel");
foreach (var child in loggingSection.GetChildren())
{
    Console.WriteLine($"  {child.Key}: {child.Value}");
}
Console.WriteLine();

// Set up DI identical to TestId.Cli's Program.cs
var services = new ServiceCollection();

services.AddLogging(builder =>
{
    builder.AddConfiguration(configuration.GetSection("Logging"));
    builder.AddConsole();
});

var testIdOptions = new TestIdOptions();
configuration.GetSection(TestIdOptions.SectionName).Bind(testIdOptions);
services.AddSingleton(testIdOptions);
services.AddSingleton<ClipboardService>();
services.AddSingleton<GitCloneService>();
services.AddSingleton<PythonScriptService>();

var serviceProvider = services.BuildServiceProvider();

// Verify logging levels on representative service loggers
Console.WriteLine("--- Log Level Verification ---");
var loggerTypes = new[]
{
    typeof(ClipboardService),
    typeof(GitCloneService),
    typeof(PythonScriptService),
};

var levels = new[]
{
    LogLevel.Trace,
    LogLevel.Debug,
    LogLevel.Information,
    LogLevel.Warning,
    LogLevel.Error,
    LogLevel.Critical,
};

var allDisabled = true;

foreach (var type in loggerTypes)
{
    var logger = serviceProvider.GetRequiredService(
        typeof(ILogger<>).MakeGenericType(type)) as ILogger;

    Console.WriteLine($"  Logger<{type.Name}>:");

    foreach (var level in levels)
    {
        var enabled = logger!.IsEnabled(level);
        Console.WriteLine($"    {level,-12}: {(enabled ? "ENABLED" : "disabled")}");
        if (enabled) allDisabled = false;
    }
}

Console.WriteLine();
Console.WriteLine(allDisabled
    ? "PASS: All log levels are disabled. No Microsoft logging will be emitted at runtime."
    : "FAIL: Some log levels are still enabled.");

serviceProvider.Dispose();

return allDisabled ? 0 : 1;
