using Microsoft.Extensions.Logging;

namespace TestId.Cli.Services;

public class ClipboardService
{
    private readonly ILogger<ClipboardService> _logger;

    public ClipboardService(ILogger<ClipboardService> logger)
    {
        _logger = logger;
    }

    public async Task SetTextAsync(string text)
    {
        try
        {
            await TextCopy.ClipboardService.SetTextAsync(text);
            _logger.LogInformation("Copied text to clipboard");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to copy text to clipboard");
        }
    }
}
