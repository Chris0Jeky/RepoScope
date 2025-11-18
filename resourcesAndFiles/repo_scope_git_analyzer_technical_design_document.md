# RepoScope – Git Repository Analyzer & Visualizer

## 0. Overview

**Name:** RepoScope  
**Tagline:** Local, offline Git repository analyzer & visualizer for developers.

RepoScope is a developer tool that analyzes Git repositories and produces:

- Rich **metrics** and **insights** (commits, authors, churn, hotspots).
- A **CLI** for generating reports and summaries.
- A **web dashboard** for interactive charts and visualizations.

It is designed as a **local, offline** tool that runs against any Git repo on disk and can later integrate with other tools (e.g., DevFoundry, Taskdeck).

This document specifies the **vision**, **feature set**, **architecture**, **data model**, **CLI**, **API**, **frontend**, and **roadmap** for RepoScope.

---

## 1. Vision & Goals

### 1.1 Problem

Developers and teams often want insights into their repositories:

- Who contributes the most and where?
- How active is the project over time?
- Which files or directories are **hotspots** (changed frequently)?
- How has the codebase evolved?

Existing tools are often:

- Cloud‑tied, requiring remote access.
- Embedded in heavy IDEs.
- Focused on just one dimension (e.g., commit history only).

### 1.2 High‑Level Goals

1. **Local & Offline**
   - Analyze any Git repository on disk without external services.

2. **Data + Visualization**
   - Provide both raw metrics (JSON/CLI) and interactive charts (web UI).

3. **Modular & Extensible**
   - New metrics and chart types can be added incrementally.
   - Core analysis library can be reused (e.g., from DevFoundry).

4. **Developer‑Friendly UX**
   - Simple CLI for quick stats.
   - Dashboards for deeper exploration.

### 1.3 Non‑Goals (MVP)

- Real‑time, always‑on monitoring of repos.
- Deep static analysis (e.g., code complexity metrics per language).
- Cloud multi‑repo aggregation.

These might come later but are not required for initial value.

---

## 2. Feature Overview

### 2.1 MVP Feature Set

Given a local Git repository path, RepoScope will:

1. **Extract commit history**
   - Total commits.
   - Commits over time (per day/week/month).

2. **Compute author metrics**
   - Commits per author.
   - Active authors over a time window.

3. **Directory/file hotspot metrics (basic)**
   - Commits per top‑level directory.
   - Optionally, top N files by change frequency.

4. **Generate output artifacts**
   - A JSON file with computed metrics.
   - A static HTML report (including charts) OR feed a small web server.

5. **CLI commands**
   - Run analysis and print a summary.
   - Generate JSON and HTML reports.

6. **Basic web dashboard**
   - View commits over time (chart).
   - View commits by author (chart).
   - View commits by directory (chart/table).

### 2.2 Near‑Term Features

- Branch‑specific analysis (e.g., only `main` or `develop`).
- Time‑window filtering (e.g., last 90 days).
- Hotspot identification (top N files by changes).
- Basic ownership metrics (who “owns” which files/directories).

### 2.3 Long‑Term Ideas

- Calendar heatmap of commits.
- Code churn metrics (lines added/removed over time).
- Bus factor analysis (risk if one author leaves).
- Integration with Git hosting APIs (GitHub/GitLab) for PR/Issue data.
- Integration with DevFoundry as a `git.stats` tool.
- Integration with Taskdeck (link board projects to repo stats).

---

## 3. System Architecture Overview

### 3.1 Architectural Style

RepoScope is structured as:

- **Core Analyzer Library** – extracts and aggregates Git data.
- **CLI** – command‑line interface for analysis and report generation.
- **Web API (optional)** – for interactive dashboard.
- **Web UI** – SPA for charts and visualizations.

### 3.2 Key Components

1. **RepoScope.Core**
   - Git access and commit enumeration.
   - Metrics computation and aggregation.
   - Data model for metrics.

2. **RepoScope.Cli**
   - Exposes core functionality through CLI.
   - Handles command‑line arguments, prints summaries.

3. **RepoScope.Api** (optional for MVP; can be combined with CLI)
   - ASP.NET Core minimal API.
   - Exposes endpoints to run analysis and serve metrics.

4. **RepoScope.Web**
   - Vue 3 SPA that visualizes metrics.
   - Can be hosted as static files reading metrics.json or served via the API.

### 3.3 High-Level Flows

**CLI Flow:**

- User runs `reposcope analyze /path/to/repo --out report/`.
- CLI calls Core Analyzer → metrics in memory.
- CLI writes `report/metrics.json` and `report/index.html` using static template + charts.

**Server/Dashboard Flow (optional):**

- API endpoint `POST /api/analyze` with body `{ "repoPath": "..." }`.
- Service runs analysis, caches metrics.
- Web UI fetches `/api/metrics` and renders charts.

MVP can start with CLI + static reports; API is a follow‑on enhancement.

---

## 4. Tech Stack Decisions

### 4.1 Core Analyzer & CLI

- **Language:** C#
- **Runtime:** .NET 8
- **Git Library:** LibGit2Sharp or similar (managed C# bindings to libgit2).
- **Project Types:**
  - Class library: `RepoScope.Core`.
  - Console app: `RepoScope.Cli`.

**Why:**

- C#/.NET provides good performance and DX for CLI tools.
- LibGit2Sharp provides a rich API for walking commits and trees.
- Easy to share logic later with ASP.NET Core API.

### 4.2 API & Web

- **API Framework:** ASP.NET Core minimal API (optional for early MVP).
- **Frontend:**
  - Vue 3 + Vite + TypeScript.
  - Chart library: e.g., Chart.js / ECharts / ApexCharts (any standard JS charting library you like).

### 4.3 Storage & Caching

- For MVP, no persistent DB.
- Metrics generated on demand and output as JSON.
- Optional: local cache using a simple JSON file (e.g., keyed by repo path and HEAD commit ID).

---

## 5. Repository & Folder Structure

Top‑level layout:

```text
reposcope/
  README.md
  .gitignore
  .editorconfig

  backend/
    RepoScope.sln
    src/
      RepoScope.Core/
      RepoScope.Cli/
      RepoScope.Api/   (optional)
    tests/
      RepoScope.Core.Tests/

  frontend/
    reposcope-web/
      (Vite + Vue + TS project)

  templates/
    report-template/
      index.html
      assets/
        js/
        css/
```

- `templates/report-template`: static HTML + JS/CSS template used by CLI to produce standalone reports.

---

## 6. Core Analyzer Design

### 6.1 Data Extraction

Use LibGit2Sharp (or similar) to:

1. Open repo at a given path.
2. Enumerate commits for specified branch (default: HEAD).
3. For each commit:
   - Id, author name/email, author date.
   - Parent commit(s).
   - Files changed (path level) and possibly stats (added/removed lines) if available.

### 6.2 Core Data Types

```csharp
namespace RepoScope.Core.Models;

public sealed class CommitInfo
{
    public string Id { get; init; } = default!;           // SHA
    public string AuthorName { get; init; } = default!;
    public string AuthorEmail { get; init; } = default!;
    public DateTimeOffset AuthorDate { get; init; };

    public IReadOnlyList<FileChangeInfo> FileChanges { get; init; } = Array.Empty<FileChangeInfo>();
}

public sealed class FileChangeInfo
{
    public string Path { get; init; } = default!;         // "src/Taskdeck/Board.cs"
    public int LinesAdded { get; init; }
    public int LinesDeleted { get; init; }
}

public sealed class RepoAnalysisOptions
{
    public string Branch { get; init; } = "HEAD";
    public DateTimeOffset? Since { get; init; }
    public DateTimeOffset? Until { get; init; }
    public int? MaxCommits { get; init; }
}
```

### 6.3 Metrics Model

The result of an analysis is a `RepoMetrics` object:

```csharp
public sealed class RepoMetrics
{
    public string RepoPath { get; init; } = default!;
    public string? Branch { get; init; }
    public string HeadCommitId { get; init; } = default!;

    public int TotalCommits { get; init; }

    public IReadOnlyList<CommitsByDay> CommitsOverTime { get; init; } = Array.Empty<CommitsByDay>();
    public IReadOnlyList<CommitsByAuthor> CommitsByAuthor { get; init; } = Array.Empty<CommitsByAuthor>();
    public IReadOnlyList<CommitsByDirectory> CommitsByDirectory { get; init; } = Array.Empty<CommitsByDirectory>();

    // Future: file-level hotspot metrics, churn, ownership.
}

public sealed class CommitsByDay
{
    public DateOnly Day { get; init; }
    public int CommitCount { get; init; }
}

public sealed class CommitsByAuthor
{
    public string AuthorName { get; init; } = default!;
    public string AuthorEmail { get; init; } = default!;
    public int CommitCount { get; init; }
}

public sealed class CommitsByDirectory
{
    public string DirectoryPath { get; init; } = default!;  // e.g., "src", "src/Core", etc.
    public int CommitCount { get; init; }
}
```

### 6.4 Analyzer Service

```csharp
public interface IRepoAnalyzer
{
    RepoMetrics Analyze(string repoPath, RepoAnalysisOptions? options = null);
}

public sealed class RepoAnalyzer : IRepoAnalyzer
{
    public RepoMetrics Analyze(string repoPath, RepoAnalysisOptions? options = null)
    {
        // 1. Open repo using LibGit2Sharp
        // 2. Enumerate commits (respecting branch, since/until, max commits)
        // 3. Build CommitInfo list
        // 4. Aggregate into RepoMetrics
        // 5. Return RepoMetrics
    }
}
```

### 6.5 Aggregation Logic (MVP)

Given `IEnumerable<CommitInfo>`:

1. **TotalCommits**: `commits.Count()`.

2. **CommitsOverTime**:
   - Group by `AuthorDate.Date` → `DateOnly`.
   - Compute counts per date; sort by date.

3. **CommitsByAuthor**:
   - Group by `(AuthorName, AuthorEmail)`.
   - Count commits.

4. **CommitsByDirectory** (basic):
   - For each `FileChangeInfo.Path`, extract top‑level directory or first 1–2 segments.
   - Group by directory and count number of commits where any file in that directory changed.

For more advanced metrics later, we can include file‑level details and code churn aggregation.

---

## 7. JSON Output & Report Generation

### 7.1 JSON Output Format

`metrics.json` contains a serialized `RepoMetrics`:

```json
{
  "repoPath": "/path/to/repo",
  "branch": "main",
  "headCommitId": "abc123...",
  "totalCommits": 1234,
  "commitsOverTime": [
    { "day": "2025-01-01", "commitCount": 5 },
    { "day": "2025-01-02", "commitCount": 3 }
  ],
  "commitsByAuthor": [
    {
      "authorName": "Alice",
      "authorEmail": "alice@example.com",
      "commitCount": 560
    }
  ],
  "commitsByDirectory": [
    {
      "directoryPath": "src",
      "commitCount": 300
    }
  ]
}
```

### 7.2 Static HTML Report

The CLI can generate a static `index.html` that:

- Embeds or fetches `metrics.json`.
- Uses a JS chart library to render:
  - Line chart: commits over time.
  - Bar chart: commits per author.
  - Bar/treemap: commits per directory.

Two options:

1. **Embed JSON directly** in HTML as a `<script>` tag (simple, one file).
2. Load `metrics.json` with `fetch` (clean separation of data and presentation).

MVP can pick option 1 for simplicity.

---

## 8. CLI Design

### 8.1 Goals

- Simple and intuitive commands.
- Works well in scripts and CI.
- Provides both human‑readable summary and machine‑readable output.

### 8.2 Command Structure

Top‑level command: `reposcope`

Subcommands:

- `reposcope analyze <path>` – run full analysis.
- `reposcope summary <path>` – quick summary to stdout.
- `reposcope report <path> --out <dir>` – generate metrics.json and index.html.

### 8.3 Options

Common options:

- `--branch <name>` – default HEAD.
- `--since <date>` – e.g., `2025-01-01`.
- `--until <date>` – upper date bound.
- `--max-commits <n>` – limit analysis.

Examples:

```bash
# Basic analysis
reposcope analyze .

# Summary of main branch commits since Jan 1st
reposcope summary . --branch main --since 2025-01-01

# Generate full report to ./report
reposcope report . --branch main --out ./report
```

### 8.4 CLI Behavior

**`analyze`**

- Runs analyzer and prints JSON to stdout (or file with `--out metrics.json`).

**`summary`**

- Runs analyzer and prints e.g.:

```text
Repo: /path/to/repo
Branch: main
Head: abc1234

Total commits: 1234
Authors: 10
Top authors:
  Alice (560 commits)
  Bob (320 commits)

Most active days:
  2025-01-01: 15 commits
  2025-01-02: 10 commits
```

**`report`**

- Runs analyzer.
- Writes `metrics.json` and `index.html` into the given output directory.
- Prints the path to `index.html`.

### 8.5 Exit Codes

- `0` – success.
- `1` – generic error.
- `2` – invalid arguments.
- `3` – not a Git repository / repo not found.

---

## 9. API Design (Optional)

### 9.1 Use Cases

- Run analysis via HTTP (for dashboards or other tools).
- Cache metrics for a repo and serve them to frontend.

### 9.2 Endpoints

Base path: `/api`.

**Run Analysis**

`POST /api/analyze`

Request:

```json
{
  "repoPath": "/path/to/repo",
  "branch": "main",
  "since": "2025-01-01",
  "until": null,
  "maxCommits": null
}
```

Response:

- Full `RepoMetrics` JSON or a reference ID for later fetches.

**Get Metrics (if caching)**

`GET /api/metrics?repoPath=/path/to/repo&branch=main`

Response:

- `RepoMetrics` for a repo/branch if previously analyzed.

MVP: could omit API and rely purely on CLI + static files.

---

## 10. Frontend / Web Dashboard Design

### 10.1 UX Goals

- High‑level overview at a glance:
  - How active is this repository?
  - Who are the main contributors?
  - Which areas of the repo are most touched?

- Simple navigation and filtering.

### 10.2 Structure (Static Report / SPA)

Two deployment approaches:

1. **Static Report (MVP):**
   - A static `index.html` that loads `metrics.json` from the same folder.
   - Charts rendered client‑side.

2. **SPA (Later):**
   - Vite + Vue project `reposcope-web`.
   - Receives `RepoMetrics` via API.

For the big design, we describe SPA; static report can be a simplified derivative.

### 10.3 Frontend Structure (SPA)

Inside `frontend/reposcope-web`:

```text
src/
  main.ts
  App.vue

  router/
    index.ts

  store/
    metricsStore.ts
    uiStore.ts

  api/
    http.ts
    metricsApi.ts

  components/
    layout/
      AppShell.vue

    charts/
      CommitsOverTimeChart.vue
      CommitsByAuthorChart.vue
      CommitsByDirectoryChart.vue

    summaries/
      RepoSummaryCard.vue

  views/
    DashboardView.vue

  types/
    metrics.ts
```

### 10.4 Dashboard View

**DashboardView.vue**

- Header: repo path, branch, head commit.
- Row 1:
  - RepoSummaryCard (total commits, authors, date range).
- Row 2:
  - CommitsOverTimeChart.
- Row 3:
  - CommitsByAuthorChart.
  - CommitsByDirectoryChart.

### 10.5 Charts

**CommitsOverTimeChart**

- X‑axis: date.
- Y‑axis: commits per day.
- Possibly allow zoom/brush for a specific date range.

**CommitsByAuthorChart**

- Horizontal bar chart.
- Sorted by commit count descending.

**CommitsByDirectoryChart**

- Bar chart of directories by commit count.
- Later: tree map by directory size/churn.

### 10.6 Data Loading Flow

- `metricsStore.loadFromJson(url)` – fetches `metrics.json` (or receives metrics object as a prop in static report mode).
- Store normalizes data if needed.
- Charts subscribe to store state.

---

## 11. Testing Strategy

### 11.1 Core Tests

- Unit tests on aggregator:
  - Grouping by day.
  - Grouping by author.
  - Grouping by directory.
- Tests for `RepoAnalyzer` with small test repos (or mocked Git data):
  - Single author repo.
  - Multi‑author repo.
  - Repo with multiple directories.

### 11.2 CLI Tests

- Test command handlers with synthetic `RepoMetrics`.
- Snapshot tests for `summary` output.

### 11.3 Frontend Tests (SPA)

- Unit tests for charts with sample data.
- Component tests for DashboardView (basic rendering).

---

## 12. Performance Considerations

- For large repos, analyzing all commits can be slow.

Mitigations:

- Allow `--max-commits` to limit how far back we go.
- Allow time range filtering via `--since/--until`.
- Use simple aggregation with streaming (don’t store too many intermediate structures if not needed).
- Cache metrics keyed by `(repoPath, branch, headCommitId, options)` where desired.

---

## 13. Integration Opportunities

### 13.1 DevFoundry Integration

- Wrap RepoScope.Core into a DevFoundry tool:
  - Tool ID: `git.repostats`.
  - Input: repo path, options.
  - Output: small summary string or JSON.

### 13.2 Taskdeck Integration

- For a given Taskdeck project/board, store a `repoPath`.
- Show selected RepoScope charts (commits over time, hotspots) inline.
- Use RepoScope metrics to inform WIP policies or highlight risky areas.

---

## 14. Roadmap & Milestones

### Phase 1 – Core & CLI

- Implement `RepoScope.Core` data types and `RepoAnalyzer` using LibGit2Sharp.
- Implement CLI with `analyze`, `summary`, and `report` commands.
- Generate `metrics.json` and a basic static `index.html` report.

### Phase 2 – Better Visualizations

- Improve static report HTML (better layout and charts).
- Add more metrics (e.g., top N files).
- Enhance CLI summary output.

### Phase 3 – SPA Dashboard & API (Optional)

- Add ASP.NET Core `RepoScope.Api`.
- Implement Vue SPA `reposcope-web` dashboard retrieving metrics from API.

### Phase 4 – Advanced Metrics & Integrations

- Add churn, hotspots, ownership metrics.
- Integrate into DevFoundry as a tool.
- Integrate into Taskdeck as a repository analytics panel.

---

## 15. Coding Guidelines & Conventions

### 15.1 C# Core & CLI

- Keep `RepoAnalyzer` and aggregation logic pure and testable.
- Place Git access behind interfaces for easier testing.
- Use `PascalCase` for types and `camelCase` for locals.

### 15.2 Frontend (if SPA)

- Use `<script setup lang="ts">`.
- Define strong types for metrics in `types/metrics.ts`.
- Keep charts as reusable components.

### 15.3 Git & Repo Workflow

- Branch naming: `feature/<description>`, `fix/<description>`.
- Keep feature branches small and scoped (e.g., `feature/author-metrics`).

---

## 16. Initial README Skeleton

```markdown
# RepoScope

RepoScope is a local, offline Git repository analyzer and visualizer.

- 🔍 Inspect commit activity over time
- 👥 See contributions per author
- 📂 Identify active directories and hotspots
- 📊 Generate JSON metrics and HTML reports

## Tech Stack

**Core & CLI**

- .NET 8 (C#)
- LibGit2Sharp (or similar)

**Optional API & Web**

- ASP.NET Core minimal API
- Vue 3 + Vite + TypeScript (for interactive dashboard)

## Getting Started

### Prerequisites

- .NET 8 SDK

### CLI

```bash
cd backend

dotnet run --project src/RepoScope.Cli/RepoScope.Cli.csproj -- analyze .
```

Generate a full report:

```bash
dotnet run --project src/RepoScope.Cli/RepoScope.Cli.csproj -- report . --out ./report
```

Open `./report/index.html` in your browser to see charts.

## Roadmap

- [ ] Core metrics: commits, authors, directories
- [ ] JSON output & static HTML reports
- [ ] CLI summary
- [ ] Advanced metrics: hotspots, churn
- [ ] Optional API + SPA dashboard
- [ ] Integrations with DevFoundry/Taskdeck

RepoScope is primarily a personal/tooling project with a focus on understanding your codebase history quickly and visually.
```

---

This design document should be updated as the implementation evolves, capturing new metrics, visualizations, and integration points.

