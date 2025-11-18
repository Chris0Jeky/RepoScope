# RepoScope

**RepoScope** is a local, offline Git repository analyzer and visualizer for developers.

Analyze any Git repository on your machine and get rich insights through interactive charts, detailed metrics, and comprehensive reports.

## Features

- 🔍 **Analyze Git Repositories** - Deep analysis of commit history, authors, and file changes
- 📊 **Interactive Visualizations** - Beautiful charts showing commits over time, contributor activity, and directory hotspots
- 📈 **Comprehensive Metrics** - Total commits, unique authors, date ranges, and more
- 💻 **CLI Tool** - Simple command-line interface for quick analysis
- 🌐 **Web Dashboard** - Modern Vue.js dashboard for interactive exploration
- 📄 **Static Reports** - Generate standalone HTML reports for offline viewing
- 🔒 **100% Local** - No cloud services, no data sent anywhere
- 🚀 **Fast & Efficient** - Powered by LibGit2Sharp for high-performance Git operations

## Quick Start

### Prerequisites

- [.NET 8 SDK](https://dotnet.net/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/) (for frontend development)

### Installation

```bash
# Clone the repository
git clone <your-repo-url>
cd RepoScope

# Build the CLI
cd backend
dotnet build
```

### Usage

#### Analyze a Repository

```bash
# Analyze current directory
dotnet run --project src/RepoScope.Cli/RepoScope.Cli.csproj -- summary .

# Analyze specific repository
dotnet run --project src/RepoScope.Cli/RepoScope.Cli.csproj -- summary /path/to/repo

# Analyze specific branch
dotnet run --project src/RepoScope.Cli/RepoScope.Cli.csproj -- summary . --branch main

# Analyze last 30 days
dotnet run --project src/RepoScope.Cli/RepoScope.Cli.csproj -- summary . --since 2025-01-01
```

#### Generate Reports

```bash
# Generate full HTML report
dotnet run --project src/RepoScope.Cli/RepoScope.Cli.csproj -- report . --out ./report

# Open the report
open ./report/index.html  # macOS
xdg-open ./report/index.html  # Linux
start ./report/index.html  # Windows
```

#### Export Metrics as JSON

```bash
# Output to stdout
dotnet run --project src/RepoScope.Cli/RepoScope.Cli.csproj -- analyze .

# Save to file
dotnet run --project src/RepoScope.Cli/RepoScope.Cli.csproj -- analyze . --out metrics.json
```

## CLI Commands

### `analyze`

Analyze a repository and output metrics as JSON.

```bash
reposcope analyze <path> [options]
```

**Options:**
- `--branch <name>` - Branch to analyze (default: HEAD)
- `--since <date>` - Only include commits after this date (ISO 8601 format)
- `--until <date>` - Only include commits before this date (ISO 8601 format)
- `--max-commits <n>` - Maximum number of commits to analyze
- `--out <file>` - Output file path (default: stdout)

### `summary`

Analyze a repository and print a human-readable summary.

```bash
reposcope summary <path> [options]
```

**Options:** Same as `analyze`

### `report`

Generate a full HTML report with interactive charts.

```bash
reposcope report <path> [options]
```

**Options:**
- Same as `analyze`, plus:
- `--out <dir>` - Output directory for report (default: ./report)

## Project Structure

```
RepoScope/
├── backend/                    # .NET backend
│   ├── src/
│   │   ├── RepoScope.Core/    # Core analysis library
│   │   │   ├── Models/        # Data models
│   │   │   └── Services/      # Analyzer service
│   │   └── RepoScope.Cli/     # CLI application
│   │       ├── Commands/      # Command implementations
│   │       └── Formatters/    # Output formatters
│   ├── tests/
│   │   └── RepoScope.Core.Tests/  # Unit tests
│   └── RepoScope.sln
│
├── frontend/                   # Vue.js frontend
│   └── reposcope-web/         # Web dashboard
│       ├── src/
│       │   ├── components/    # Vue components
│       │   ├── views/         # Page views
│       │   ├── store/         # State management
│       │   └── types/         # TypeScript types
│       └── package.json
│
└── templates/                  # Report templates
    └── report-template/       # Static HTML template
        ├── index.html
        └── assets/
            ├── css/
            └── js/
```

## Architecture

### Core Library (`RepoScope.Core`)

The heart of RepoScope is the `RepoAnalyzer` service, which:

1. Opens a Git repository using **LibGit2Sharp**
2. Enumerates commits based on filters (branch, date range, max count)
3. Extracts file changes and line statistics
4. Aggregates metrics into structured data

**Key Models:**
- `RepoMetrics` - Complete analysis results
- `CommitInfo` - Individual commit data
- `CommitsByDay/Author/Directory` - Aggregated views

### CLI (`RepoScope.Cli`)

Built with **System.CommandLine**, the CLI provides:
- Command-line argument parsing
- Multiple output formats (JSON, summary, HTML)
- Error handling and validation

### Web Dashboard (`reposcope-web`)

Modern Vue 3 + TypeScript SPA featuring:
- **Chart.js** for interactive visualizations
- Reactive state management
- Responsive design
- Loads metrics.json for standalone deployment

## Metrics Computed

### Overview Metrics
- Total commits
- Unique authors
- Date range (earliest/latest commits)
- Active days count

### Commits Over Time
- Daily commit counts
- Time series visualization

### Commits by Author
- Per-author commit counts
- Contribution percentages
- Email addresses

### Commits by Directory
- Top-level directory activity
- Hotspot identification

### File-Level Hotspots
- Files with the most commits
- Lines added/deleted per file
- Total code churn per file
- Identify high-volatility files

### Code Churn Over Time
- Lines added/deleted by day
- Net change tracking
- Visualize development activity patterns
- Identify periods of high activity

## Development

### Backend Development

```bash
cd backend

# Restore dependencies
dotnet restore

# Build
dotnet build

# Run tests
dotnet test

# Run CLI
dotnet run --project src/RepoScope.Cli/RepoScope.Cli.csproj -- summary .
```

### Frontend Development

```bash
cd frontend/reposcope-web

# Install dependencies
npm install

# Start dev server
npm run dev

# Build for production
npm run build
```

### Running Tests

```bash
cd backend
dotnet test
```

## Use Cases

### For Individual Developers
- Understand your contribution patterns
- Identify areas of the codebase you work on most
- Track activity over time

### For Teams
- See who contributes what and where
- Identify knowledge concentration risks
- Understand project evolution

### For Project Managers
- Track team velocity and activity
- Generate reports for stakeholders
- Identify bottlenecks and hotspots

## Roadmap

### MVP (Current)
- [x] Core repository analysis
- [x] CLI with analyze/summary/report commands
- [x] Static HTML reports with charts
- [x] Vue.js dashboard
- [x] Basic unit tests
- [x] Improved HTML report template integration
- [x] File-level hotspot analysis
- [x] Code churn metrics (lines added/removed over time)

### Near-Term
- [ ] Calendar heatmap visualization
- [ ] Configurable analysis options
- [ ] Performance optimizations for large repositories

### Future
- [ ] ASP.NET Core API for dashboard
- [ ] Real-time analysis mode
- [ ] Multiple repository comparison
- [ ] Bus factor analysis
- [ ] Integration with GitHub/GitLab APIs
- [ ] DevFoundry tool integration
- [ ] Taskdeck integration

## Technical Details

### Technologies

**Backend:**
- .NET 8 (C#)
- LibGit2Sharp - Git operations
- System.CommandLine - CLI framework
- xUnit - Testing

**Frontend:**
- Vue 3 - UI framework
- TypeScript - Type safety
- Vite - Build tool
- Chart.js - Visualizations

### Performance Considerations

- **Large Repositories**: Use `--max-commits` to limit analysis scope
- **Date Filtering**: Use `--since` and `--until` for specific time windows
- **Branch Selection**: Analyze specific branches to reduce scope

### Data Privacy

RepoScope is 100% local and offline:
- No data is sent to any external service
- All analysis happens on your machine
- Generated reports are static files you control

## Contributing

Contributions are welcome! This is a personal project but feedback and improvements are appreciated.

### Development Setup

1. Install .NET 8 SDK
2. Install Node.js 18+
3. Clone the repository
4. Build and run tests

### Code Style

- C#: Follow standard .NET conventions
- TypeScript/Vue: ESLint/Prettier configurations
- Commit messages: Conventional Commits format

## License

MIT License - See LICENSE file for details

## Acknowledgments

- Built with [LibGit2Sharp](https://github.com/libgit2/libgit2sharp)
- Charts powered by [Chart.js](https://www.chartjs.org/)
- UI framework: [Vue.js](https://vuejs.org/)

---

**RepoScope** - Understand your code, visualize your history