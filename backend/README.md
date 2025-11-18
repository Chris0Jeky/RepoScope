# RepoScope Backend

.NET backend for RepoScope repository analyzer.

## Projects

### RepoScope.Core

Core analysis library containing:
- Data models (`Models/`)
- Repository analyzer service (`Services/`)

**Key Components:**
- `RepoAnalyzer` - Main analyzer using LibGit2Sharp
- `RepoMetrics` - Aggregated metrics model
- `CommitInfo` - Individual commit data

### RepoScope.Cli

Command-line interface built with System.CommandLine.

**Commands:**
- `analyze` - Output JSON metrics
- `summary` - Print human-readable summary
- `report` - Generate HTML report

### RepoScope.Core.Tests

Unit tests using xUnit.

## Building

```bash
# Restore dependencies
dotnet restore

# Build all projects
dotnet build

# Run tests
dotnet test

# Run CLI
dotnet run --project src/RepoScope.Cli/RepoScope.Cli.csproj -- summary .
```

## Architecture

The backend follows a clean architecture:

1. **Models** - Pure data classes
2. **Services** - Business logic and Git operations
3. **CLI** - User interface layer

## Dependencies

- **LibGit2Sharp** - Git repository access
- **System.CommandLine** - CLI framework
- **xUnit** - Testing framework

## Development

Requires .NET 8 SDK. Follow standard .NET coding conventions.
