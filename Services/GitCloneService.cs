using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace TestId.Services;

public class GitCloneService
{
    private readonly ILogger<GitCloneService> _logger;

    public GitCloneService(ILogger<GitCloneService> logger)
    {
        _logger = logger;
    }

    public async Task<string> CloneRepositoryAsync(string repositoryUrl, string commitSha, CancellationToken cancellationToken = default)
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"testid_{Guid.NewGuid()}");
        
        try
        {
            _logger.LogInformation("Cloning repository {Repository} at commit {Commit} to {Path}", 
                repositoryUrl, commitSha, tempPath);

            // Clone the repository
            await ExecuteGitCommandAsync($"clone {repositoryUrl} \"{tempPath}\"", null, cancellationToken);

            // Checkout the specific commit
            await ExecuteGitCommandAsync($"checkout {commitSha}", tempPath, cancellationToken);

            _logger.LogInformation("Successfully cloned repository to {Path}", tempPath);
            
            return tempPath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clone repository {Repository}", repositoryUrl);
            
            // Clean up on failure
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }
            
            throw;
        }
    }

    public void CleanupRepository(string repositoryPath)
    {
        try
        {
            if (Directory.Exists(repositoryPath))
            {
                _logger.LogInformation("Cleaning up repository at {Path}", repositoryPath);
                Directory.Delete(repositoryPath, true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to cleanup repository at {Path}", repositoryPath);
        }
    }

    private async Task ExecuteGitCommandAsync(string arguments, string? workingDirectory, CancellationToken cancellationToken)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "git",
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        if (!string.IsNullOrEmpty(workingDirectory))
        {
            startInfo.WorkingDirectory = workingDirectory;
        }

        using var process = new Process { StartInfo = startInfo };
        
        var outputBuilder = new System.Text.StringBuilder();
        var errorBuilder = new System.Text.StringBuilder();

        process.OutputDataReceived += (sender, e) =>
        {
            if (e.Data != null)
            {
                outputBuilder.AppendLine(e.Data);
            }
        };

        process.ErrorDataReceived += (sender, e) =>
        {
            if (e.Data != null)
            {
                errorBuilder.AppendLine(e.Data);
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync(cancellationToken);

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"Git command 'git {arguments}' failed with exit code {process.ExitCode}. " +
                $"Error: {errorBuilder}");
        }
    }
}
