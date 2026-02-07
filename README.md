# TestId

A C# CLI application for generating test IDs from Git repositories using Python scripts.

## Overview

TestId is a command-line tool that clones a Git repository at a specific commit, executes a Python script within that repository to generate test IDs, and displays the results in the console. Generated IDs are automatically copied to the clipboard. The tool automatically cleans up temporary files after execution.

## Features

- **Git Integration**: Clones repositories and checks out specific commits
- **Python Script Execution**: Runs Python scripts from the cloned repository
- **Multiple ID Generation**: Generate multiple test IDs with a single command using the `-n` option
- **Test ID Kinds**: Generate unit test IDs (`-k U`) or acceptance test IDs (`-k C`)
- **Clipboard Support**: Generated test IDs are automatically copied to the clipboard
- **Dependency Injection**: Built with Microsoft Extensions for DI, Logging, and Configuration
- **Structured Logging**: Comprehensive logging using Microsoft.Extensions.Logging
- **Automatic Cleanup**: Temporary repositories are automatically removed after execution

## Prerequisites

- .NET 8.0 SDK
- Git command-line tool
- Python 3.x

## Installation

1. Clone this repository:
```bash
git clone https://github.com/QuinntyneBrown/TestId.git
cd TestId
```

2. Build the project:
```bash
dotnet build
```

3. Install as a global tool:
```bash
dotnet pack
dotnet tool install --global --add-source ./src/TestId.Cli/nupkg TestId.Cli
```

4. Run the application:
```bash
test-id generate [options]
```

## Usage

### Basic Command

Generate a single unit test ID from a repository:
```bash
test-id generate -r <repository-url> -c <commit-sha>
```

### Generate Multiple Test IDs

Use the `-n` or `--number` option to generate multiple test IDs:
```bash
test-id generate -r <repository-url> -c <commit-sha> -n 20
```

This will clone the repository once and call the Python script 20 times to generate 20 test IDs.

### Test ID Kinds

Use the `-k` or `--kind` option to specify the type of test ID:

- `U` (default): Unit test IDs (prefixed with `UT-`)
- `C`: Acceptance test IDs (prefixed with `AT-`)

```bash
test-id generate -r <repository-url> -c <commit-sha> -k C
```

### Command Options

- `-r, --repository <url>` (REQUIRED): The Git repository URL to clone
- `-c, --commit <sha>` (REQUIRED): The commit SHA to checkout
- `-n, --number <count>`: Number of test IDs to generate (default: 1)
- `-k, --kind <U|C>`: Kind of test ID — U for unit test, C for acceptance test (default: U)
- `-h, --help`: Show help and usage information

### Examples

Generate 1 unit test ID:
```bash
test-id generate -r https://github.com/QuinntyneBrown/TestId.git -c cd1f47c
```

Generate 20 unit test IDs:
```bash
test-id generate -r https://github.com/QuinntyneBrown/TestId.git -c cd1f47c -n 20
```

Generate 5 acceptance test IDs:
```bash
test-id generate -r https://github.com/QuinntyneBrown/TestId.git -c cd1f47c -n 5 -k C
```

## Project Structure

```
TestId/
├── src/
│   └── TestId.Cli/
│       ├── Commands/
│       │   └── GenerateCommand.cs       # Command implementation
│       ├── Services/
│       │   ├── ClipboardService.cs      # Clipboard operations service
│       │   ├── GitCloneService.cs       # Git operations service
│       │   └── PythonScriptService.cs   # Python script execution service
│       ├── Options/
│       │   └── TestIdOptions.cs         # Configuration options
│       ├── scripts/
│       │   └── generate_test_id.py      # Python script for generating test IDs
│       ├── Program.cs                   # Application entry point
│       ├── TestId.Cli.csproj            # Project file
│       └── appsettings.json             # Configuration file
├── eng/                                 # Engineering/build configuration
└── TestId.sln                           # Solution file
```

## Configuration

The application can be configured using `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "System": "Warning"
    }
  },
  "TestId": {
    "PythonScriptPath": "scripts/generate_test_id.py"
  }
}
```

## Python Script

The included Python script (`scripts/generate_test_id.py`) generates UUID-based test IDs prefixed by kind. It accepts a `-kind` argument:

- `-kind U` produces IDs like `UT-<uuid>`
- `-kind C` produces IDs like `AT-<uuid>`

You can replace this with your own script that implements custom test ID generation logic. The script should:

1. Be executable by Python 3
2. Accept a `-kind` argument with values `U` or `C`
3. Output the test ID to stdout
4. Exit with code 0 on success

## Technologies Used

- **System.CommandLine**: Command-line parsing and handling
- **Microsoft.Extensions.DependencyInjection**: Dependency injection container
- **Microsoft.Extensions.Logging**: Structured logging
- **Microsoft.Extensions.Configuration**: Configuration management
- **TextCopy**: Cross-platform clipboard support
- **.NET 8.0**: Target framework

## License

MIT License