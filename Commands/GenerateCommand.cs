using Microsoft.Extensions.Logging;
using System.CommandLine;
using TestId.Options;
using TestId.Services;

namespace TestId.Commands;

public class GenerateCommand : Command
{
    private readonly GitCloneService _gitCloneService;
    private readonly PythonScriptService _pythonScriptService;
    private readonly ILogger<GenerateCommand> _logger;
    private readonly TestIdOptions _options;

    public GenerateCommand(
        GitCloneService gitCloneService,
        PythonScriptService pythonScriptService,
        ILogger<GenerateCommand> logger,
        TestIdOptions options) 
        : base("generate", "Generates test IDs from a cloned repository")
    {
        _gitCloneService = gitCloneService;
        _pythonScriptService = pythonScriptService;
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

        AddOption(repositoryOption);
        AddOption(commitOption);
        AddOption(countOption);

        this.SetHandler(async (repository, commit, count) =>
        {
            await ExecuteAsync(repository, commit, count, CancellationToken.None);
        }, repositoryOption, commitOption, countOption);
    }

    private async Task ExecuteAsync(
        string repository,
        string commit,
        int count,
        CancellationToken cancellationToken)
    {
        string? repositoryPath = null;

        try
        {
            // Clone the repository
            repositoryPath = await _gitCloneService.CloneRepositoryAsync(repository, commit, cancellationToken);

            // Generate test IDs
            for (int i = 0; i < count; i++)
            {
                try
                {
                    var testId = await _pythonScriptService.ExecuteScriptAsync(
                        repositoryPath, 
                        _options.PythonScriptPath, 
                        cancellationToken);

                    Console.WriteLine($"Test ID {i + 1}: {testId}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to generate test ID {Index}", i + 1);
                    throw;
                }
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
