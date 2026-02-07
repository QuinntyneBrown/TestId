using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace TestId.Services;

public class PythonScriptService
{
    private readonly ILogger<PythonScriptService> _logger;

    public PythonScriptService(ILogger<PythonScriptService> logger)
    {
        _logger = logger;
    }

    public async Task<string> ExecuteScriptAsync(string repositoryPath, string scriptPath, CancellationToken cancellationToken = default)
    {
        var fullScriptPath = Path.Combine(repositoryPath, scriptPath);
        
        if (!File.Exists(fullScriptPath))
        {
            throw new FileNotFoundException($"Python script not found at {fullScriptPath}");
        }

        _logger.LogInformation("Executing Python script: {ScriptPath}", fullScriptPath);

        var startInfo = new ProcessStartInfo
        {
            FileName = "python3",
            Arguments = $"\"{fullScriptPath}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = repositoryPath
        };

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
            var error = errorBuilder.ToString();
            _logger.LogError("Python script failed with exit code {ExitCode}. Error: {Error}", 
                process.ExitCode, error);
            throw new InvalidOperationException(
                $"Python script execution failed with exit code {process.ExitCode}. Error: {error}");
        }

        var output = outputBuilder.ToString().Trim();
        _logger.LogInformation("Python script completed successfully. Output: {Output}", output);
        
        return output;
    }
}
