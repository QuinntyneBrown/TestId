# TestId

A C# CLI application for generating test IDs from Git repositories using Python scripts.

## Overview

TestId is a command-line tool that clones a Git repository at a specific commit, executes a Python script within that repository to generate test IDs, and displays the results in the console. The tool automatically cleans up temporary files after execution.

## Features

- **Git Integration**: Clones repositories and checks out specific commits
- **Python Script Execution**: Runs Python scripts from the cloned repository
- **Multiple ID Generation**: Generate multiple test IDs with a single command using the `-n` option
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

3. Run the application:
```bash
dotnet run -- generate [options]
```

## Usage

### Basic Command

Generate a single test ID from a repository:
```bash
dotnet run -- generate -r <repository-url> -c <commit-sha>
```

### Generate Multiple Test IDs

Use the `-n` or `--number` option to generate multiple test IDs:
```bash
dotnet run -- generate -r <repository-url> -c <commit-sha> -n 20
```

This will clone the repository once and call the Python script 20 times to generate 20 test IDs.

### Command Options

- `-r, --repository <url>` (REQUIRED): The Git repository URL to clone
- `-c, --commit <sha>` (REQUIRED): The commit SHA to checkout
- `-n, --number <count>`: Number of test IDs to generate (default: 1)
- `-h, --help`: Show help and usage information

### Examples

Generate 1 test ID:
```bash
dotnet run -- generate -r https://github.com/QuinntyneBrown/TestId.git -c cd1f47c
```

Generate 20 test IDs:
```bash
dotnet run -- generate -r https://github.com/QuinntyneBrown/TestId.git -c cd1f47c -n 20
```

## Project Structure

```
TestId/
├── Commands/
│   └── GenerateCommand.cs       # Command implementation
├── Services/
│   ├── GitCloneService.cs       # Git operations service
│   └── PythonScriptService.cs   # Python script execution service
├── Options/
│   └── TestIdOptions.cs         # Configuration options
├── scripts/
│   └── generate_test_id.py      # Sample Python script for generating test IDs
├── Program.cs                   # Application entry point
├── TestId.csproj                # Project file
└── appsettings.json             # Configuration file
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

The included Python script (`scripts/generate_test_id.py`) generates UUID-based test IDs. You can replace this with your own script that implements custom test ID generation logic. The script should:

1. Be executable by Python 3
2. Output the test ID to stdout
3. Exit with code 0 on success

Example custom script:
```python
#!/usr/bin/env python3
import uuid
import sys

def generate_test_id():
    test_id = str(uuid.uuid4())
    return test_id

if __name__ == "__main__":
    test_id = generate_test_id()
    print(test_id)
    sys.exit(0)
```

## Technologies Used

- **System.CommandLine**: Command-line parsing and handling
- **Microsoft.Extensions.DependencyInjection**: Dependency injection container
- **Microsoft.Extensions.Logging**: Structured logging
- **Microsoft.Extensions.Configuration**: Configuration management
- **.NET 8.0**: Target framework

## License

MIT License