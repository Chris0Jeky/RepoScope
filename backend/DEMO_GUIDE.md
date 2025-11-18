# RepoScope Demo Guide

## Quick Demo Steps

### 1. Build the Project (already done)
```bash
dotnet build
```

### 2. Run Tests (all passing)
```bash
dotnet test
# Result: 6 tests passed
```

### 3. Demo CLI Commands

#### a) Quick Summary
Show a human-readable summary of any repository:
```bash
dotnet run --project src/RepoScope.Cli/RepoScope.Cli.csproj -- summary .
```

**What it shows:**
- Total commits and unique authors
- Top contributors with percentages
- Most active directories
- Most active days

#### b) JSON Analysis
Get detailed metrics in JSON format:
```bash
dotnet run --project src/RepoScope.Cli/RepoScope.Cli.csproj -- analyze . --out analysis.json
```

**Output includes:**
- Commits over time (daily breakdown)
- Commits by author
- Commits by directory
- **File hotspots** (most frequently changed files)
- **Code churn metrics** (lines added/deleted over time)

#### c) Generate HTML Report
Create a beautiful, interactive HTML report:
```bash
dotnet run --project src/RepoScope.Cli/RepoScope.Cli.csproj -- report . --out demo-output
```

Then open `demo-output/index.html` in your browser!

### 4. What's in the HTML Report?

The HTML report includes:

**Summary Section:**
- Repository information (path, branch, commit ID)
- Key metrics cards (total commits, contributors, directories, active days)

**Visualizations:**
- **Commits Over Time** - Line chart showing commit activity
- **Top Contributors** - Horizontal bar chart
- **Most Active Directories** - Bar chart
- **Code Churn Over Time** - Stacked area chart showing lines added/deleted
- **File Hotspots Table** - Shows which files are changed most frequently with churn metrics

**Tables:**
- All Contributors (with percentages)
- Directory Activity (with percentages)
- File Hotspots (detailed metrics: commits, lines added/deleted, total churn)

### 5. Advanced Options

#### Filter by Branch
```bash
dotnet run --project src/RepoScope.Cli/RepoScope.Cli.csproj -- summary . --branch main
```

#### Filter by Date Range
```bash
dotnet run --project src/RepoScope.Cli/RepoScope.Cli.csproj -- summary . --since 2025-01-01
```

#### Limit Number of Commits
```bash
dotnet run --project src/RepoScope.Cli/RepoScope.Cli.csproj -- summary . --max-commits 100
```

## Demo Script (2-3 minutes)

1. **Introduce RepoScope**
   - "RepoScope is a local, offline Git repository analyzer"
   - "It analyzes any Git repo and provides insights through CLI and visualizations"

2. **Show Quick Summary**
   - Run the `summary` command
   - Point out: total commits, top contributors, active directories
   - Highlight: "This gives you a quick overview without leaving the terminal"

3. **Generate HTML Report**
   - Run the `report` command
   - Open the HTML file in browser
   - Walk through the visualizations:
     - Commits over time chart
     - Contributors breakdown
     - Directory activity
     - **Code churn over time** (show velocity of development)
     - **File hotspots** (identify problematic files)

4. **Highlight Key Features**
   - "Completely local and offline - no cloud required"
   - "Works with any Git repository"
   - "Helps identify hotspots - files that change frequently may need refactoring"
   - "Code churn metrics show development velocity"
   - "Can filter by branch, date range, or commit count"

5. **Next Steps**
   - "Built with .NET 8 and LibGit2Sharp for fast Git analysis"
   - "Frontend uses Vue 3 for the SPA version (future enhancement)"
   - "Can integrate with other tools like DevFoundry or Taskdeck"

## Test Data

The demo-output folder contains a pre-generated report analyzing the RepoScope repository itself:
- 14 commits total
- 3 contributors (Claude, Cristian Tcaci, Chris0Jeky)
- 5 main directories analyzed
- Shows file-level hotspots and code churn metrics
- 8,104 lines added, 20 deleted in the analyzed period

## Technical Highlights

**Clean Architecture:**
- RepoScope.Core: Pure analysis library
- RepoScope.Cli: Command-line interface
- RepoScope.Api: (Future) REST API for web dashboard

**Key Innovations:**
- Single-pass analysis for efficiency
- File-level hotspot detection
- Code churn tracking over time
- Immutable data models
- Comprehensive test coverage

**Technologies:**
- .NET 8 / C#
- LibGit2Sharp for Git access
- Chart.js for visualizations
- System.CommandLine for CLI
- xUnit for testing
