# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build Commands

```bash
# Restore dependencies
dotnet restore

# Build all projects
dotnet build

# Build in release mode
dotnet build -c Release

# Run tests
dotnet test

# Run specific test
dotnet test --filter "FullyQualifiedName~RepoAnalyzerTests.Analyze_WithNullPath_ThrowsArgumentException"

# Run CLI (from backend directory)
dotnet run --project src/RepoScope.Cli/RepoScope.Cli.csproj -- [command] [options]
```

## CLI Commands

```bash
# Analyze repository and output JSON
dotnet run --project src/RepoScope.Cli/RepoScope.Cli.csproj -- analyze [path]

# Output human-readable summary
dotnet run --project src/RepoScope.Cli/RepoScope.Cli.csproj -- summary [path]

# Generate HTML report
dotnet run --project src/RepoScope.Cli/RepoScope.Cli.csproj -- report [path] --out report.html

# Common options for all commands:
# --branch <name>          Branch to analyze (default: HEAD)
# --since <date>           Only commits after this date (ISO 8601)
# --until <date>           Only commits before this date (ISO 8601)
# --max-commits <n>        Maximum number of commits to analyze
```

## Architecture

RepoScope is a .NET 8 repository analyzer with three main projects:

### RepoScope.Core (src/RepoScope.Core/)

Core analysis library with no dependencies on CLI or presentation logic.

**Key Services:**
- `IRepoAnalyzer` - Interface for repository analysis
- `RepoAnalyzer` - Implementation using LibGit2Sharp to traverse Git history

**Analysis Flow:**
1. `Analyze()` validates repo path and opens repository
2. `GetBranch()` resolves branch reference (supports "HEAD" or named branches)
3. `ExtractCommits()` queries commits with filtering (date range, max commits)
4. `ExtractFileChanges()` compares each commit with its parent to get diffs
5. `AggregateMetrics()` processes all commits to compute aggregated statistics

**Data Models:**
- `RepoMetrics` - Top-level aggregated metrics container
- `CommitInfo` - Individual commit with file changes
- `FileChangeInfo` - Lines added/deleted per file
- `CommitsByDay`, `CommitsByAuthor`, `CommitsByDirectory` - Time series and grouping models
- `FileHotspot` - File-level activity metrics (commit count, churn)
- `CodeChurnByDay` - Daily code change metrics

**Options:**
- `RepoAnalysisOptions` - Controls branch, date filters, and commit limits

### RepoScope.Cli (src/RepoScope.Cli/)

Command-line interface using System.CommandLine framework.

**Commands Structure:**
- `Program.cs` - Root command setup
- `Commands/AnalyzeCommand.cs` - JSON output
- `Commands/SummaryCommand.cs` - Human-readable summary
- `Commands/ReportCommand.cs` - HTML report generation

**Formatters:**
- `JsonFormatter` - Serializes RepoMetrics to JSON
- `SummaryFormatter` - Text-based summary output

### RepoScope.Core.Tests (tests/RepoScope.Core.Tests/)

Unit tests using xUnit framework. Tests validate error handling, null checks, and invalid input scenarios.

## Key Design Patterns

**Immutable Data Models:** All models use `init` accessors and readonly collections to ensure metrics are immutable after creation.

**Interface-Based Services:** `IRepoAnalyzer` allows for testing and potential alternative implementations beyond LibGit2Sharp.

**Command Pattern:** Each CLI command is a separate class that creates and configures System.CommandLine `Command` objects.

**Single-Pass Analysis:** The analyzer makes one traversal through commit history, computing all metrics in a single pass for efficiency.

## Important Conventions

- Target framework: .NET 8
- Nullable reference types enabled
- Implicit usings enabled
- Follow standard .NET naming conventions
- All public APIs have XML documentation comments
- Models use sealed classes with init-only properties
- Use DateTimeOffset for timestamps (LibGit2Sharp provides these)
- Use DateOnly for daily aggregations
- Top-level directory extraction splits on both '/' and '\' for cross-platform support
