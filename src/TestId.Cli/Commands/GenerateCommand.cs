using Microsoft.Extensions.Logging;
using System.CommandLine;
using TestId.Cli.Options;
using TestId.Cli.Services;

namespace TestId.Cli.Commands;

public class GenerateCommand : Command
{
    private readonly GitCloneService _gitCloneService;
    private readonly PythonScriptService _pythonScriptService;
    private readonly ClipboardService _clipboardService;
    private readonly ILogger<GenerateCommand> _logger;
    private readonly TestIdOptions _options;

    public GenerateCommand(
        GitCloneService gitCloneService,
        PythonScriptService pythonScriptService,
        ClipboardService clipboardService,
        ILogger<GenerateCommand> logger,
        TestIdOptions options)
        : base("generate", "Generates test IDs from a cloned repository")
    {
        _gitCloneService = gitCloneService;
        _pythonScriptService = pythonScriptService;
        _clipboardService = clipboardService;
        _logger = logger;
        _options = options;

        var repositoryOption = new Option<string>(
            aliases: new[] { "--repository", "-r" },
            description: "The Git repository URL to clone")
        {
            IsRequired = true
        };

        var commitOption = new Option<string>(
            aliases: new[] { "--commit", "-c" },
            description: "The commit SHA to checkout")
        {
            IsRequired = true
        };

        var countOption = new Option<int>(
            aliases: new[] { "--number", "-n" },
            getDefaultValue: () => 1,
            description: "Number of test IDs to generate");

        var kindOption = new Option<string>(
            aliases: new[] { "--kind", "-k" },
            getDefaultValue: () => "U",
            description: "Kind of test ID: U for unit test, C for acceptance test");
        kindOption.FromAmong("U", "C");

        AddOption(repositoryOption);
        AddOption(commitOption);
        AddOption(countOption);
        AddOption(kindOption);

        this.SetHandler(async (repository, commit, count, kind) =>
        {
            await ExecuteAsync(repository, commit, count, kind, CancellationToken.None);
        }, repositoryOption, commitOption, countOption, kindOption);
    }

    private async Task ExecuteAsync(
        string repository,
        string commit,
        int count,
        string kind,
        CancellationToken cancellationToken)
    {
        string? repositoryPath = null;

        try
        {
            // Clone the repository
            repositoryPath = await _gitCloneService.CloneRepositoryAsync(repository, commit, cancellationToken);

            // Generate test IDs
            var testIds = new List<string>();

            for (int i = 0; i < count; i++)
            {
                try
                {
                    var testId = await _pythonScriptService.ExecuteScriptAsync(
                        repositoryPath,
                        _options.PythonScriptPath,
                        kind,
                        cancellationToken);

                    testIds.Add(testId);
                    Console.WriteLine($"Test ID {i + 1}: {testId}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to generate test ID {Index}", i + 1);
                    throw;
                }
            }

            // Copy generated test IDs to clipboard
            if (testIds.Count > 0)
            {
                var clipboardText = string.Join(Environment.NewLine, testIds);
                await _clipboardService.SetTextAsync(clipboardText);
                Console.WriteLine("Test IDs copied to clipboard.");
            }
        }
        finally
        {
            // Clean up the cloned repository
            if (repositoryPath != null)
            {
                _gitCloneService.CleanupRepository(repositoryPath);
            }
        }
    }
}
