using System.CommandLine;
using RepoScope.Cli.Formatters;
using RepoScope.Core.Models;
using RepoScope.Core.Services;

namespace RepoScope.Cli.Commands;

/// <summary>
/// Command to analyze a repository and generate a full HTML report.
/// </summary>
public static class ReportCommand
{
    public static Command Create()
    {
        var command = new Command("report", "Analyze a Git repository and generate an HTML report");

        var pathArg = new Argument<string>(
            "path",
            description: "Path to the Git repository",
            getDefaultValue: () => Directory.GetCurrentDirectory());

        var branchOpt = new Option<string>(
            "--branch",
            description: "Branch to analyze",
            getDefaultValue: () => "HEAD");

        var sinceOpt = new Option<DateTimeOffset?>(
            "--since",
            description: "Only include commits after this date (ISO 8601 format)");

        var untilOpt = new Option<DateTimeOffset?>(
            "--until",
            description: "Only include commits before this date (ISO 8601 format)");

        var maxCommitsOpt = new Option<int?>(
            "--max-commits",
            description: "Maximum number of commits to analyze");

        var outputOpt = new Option<string>(
            "--out",
            description: "Output directory for report",
            getDefaultValue: () => "./report");

        command.AddArgument(pathArg);
        command.AddOption(branchOpt);
        command.AddOption(sinceOpt);
        command.AddOption(untilOpt);
        command.AddOption(maxCommitsOpt);
        command.AddOption(outputOpt);

        command.SetHandler(async (path, branch, since, until, maxCommits, output) =>
        {
            try
            {
                var analyzer = new RepoAnalyzer();
                var options = new RepoAnalysisOptions
                {
                    Branch = branch,
                    Since = since,
                    Until = until,
                    MaxCommits = maxCommits
                };

                Console.WriteLine($"Analyzing repository at {path}...");
                var metrics = analyzer.Analyze(path, options);

                // Create output directory
                Directory.CreateDirectory(output);

                // Write metrics.json
                var metricsPath = Path.Combine(output, "metrics.json");
                JsonFormatter.WriteToFile(metrics, metricsPath);
                Console.WriteLine($"Metrics written to {metricsPath}");

                // Copy or generate index.html
                var htmlPath = Path.Combine(output, "index.html");
                await GenerateHtmlReport(htmlPath);
                Console.WriteLine($"HTML report written to {htmlPath}");

                Console.WriteLine();
                Console.WriteLine($"Report generated successfully!");
                Console.WriteLine($"Open {htmlPath} in your browser to view the report.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                Environment.Exit(1);
            }
        }, pathArg, branchOpt, sinceOpt, untilOpt, maxCommitsOpt, outputOpt);

        return command;
    }

    private static async Task GenerateHtmlReport(string outputPath)
    {
        // Find the template directory relative to the solution
        var templateSourcePath = FindTemplatePath();

        if (templateSourcePath != null && Directory.Exists(templateSourcePath))
        {
            // Copy template files
            await CopyTemplateFiles(templateSourcePath, Path.GetDirectoryName(outputPath)!);
        }
        else
        {
            // Fallback to embedded template if template directory not found
            Console.WriteLine("Warning: Template directory not found, using embedded template");
            var html = GetEmbeddedHtmlTemplate();
            await File.WriteAllTextAsync(outputPath, html);
        }
    }

    private static string? FindTemplatePath()
    {
        // Try to find templates/report-template from current directory upwards
        var currentDir = Directory.GetCurrentDirectory();
        var maxLevelsUp = 5;

        for (int i = 0; i < maxLevelsUp; i++)
        {
            var templatePath = Path.Combine(currentDir, "templates", "report-template");
            if (Directory.Exists(templatePath))
            {
                return templatePath;
            }

            var parentDir = Directory.GetParent(currentDir);
            if (parentDir == null) break;
            currentDir = parentDir.FullName;
        }

        return null;
    }

    private static async Task CopyTemplateFiles(string sourceDir, string destDir)
    {
        // Copy index.html
        var indexSource = Path.Combine(sourceDir, "index.html");
        var indexDest = Path.Combine(destDir, "index.html");
        File.Copy(indexSource, indexDest, overwrite: true);

        // Copy assets directory
        var assetsSourceDir = Path.Combine(sourceDir, "assets");
        var assetsDestDir = Path.Combine(destDir, "assets");

        if (Directory.Exists(assetsSourceDir))
        {
            await CopyDirectory(assetsSourceDir, assetsDestDir);
        }
    }

    private static async Task CopyDirectory(string sourceDir, string destDir)
    {
        Directory.CreateDirectory(destDir);

        // Copy all files
        foreach (var file in Directory.GetFiles(sourceDir))
        {
            var destFile = Path.Combine(destDir, Path.GetFileName(file));
            File.Copy(file, destFile, overwrite: true);
        }

        // Copy all subdirectories
        foreach (var subDir in Directory.GetDirectories(sourceDir))
        {
            var destSubDir = Path.Combine(destDir, Path.GetFileName(subDir));
            await CopyDirectory(subDir, destSubDir);
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// Fallback used when the template directory cannot be found. It is deliberately a no-chart,
    /// no-network page: tables only, every repository-derived value is set with textContent, and a
    /// CSP blocks anything that is not the page itself. (Vendoring the 200 KB chart bundle into a
    /// string would cost far more than the tables do.)
    /// </summary>
    public static string GetEmbeddedHtmlTemplate()
    {
        return """
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta http-equiv="Content-Security-Policy" content="default-src 'none'; script-src 'unsafe-inline'; style-src 'unsafe-inline'; connect-src 'self'">
    <title>RepoScope Report</title>
    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; }
        body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; padding: 20px; background: #f5f5f5; }
        .container { max-width: 1200px; margin: 0 auto; }
        h1 { color: #333; margin-bottom: 10px; }
        h2 { margin-bottom: 10px; }
        .header, .section { background: white; padding: 30px; border-radius: 8px; margin-bottom: 20px; box-shadow: 0 1px 3px rgba(0,0,0,0.1); }
        .meta { color: #666; font-size: 14px; }
        .meta div { overflow-wrap: anywhere; }
        .cards { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 20px; margin-bottom: 20px; }
        .card { background: white; padding: 20px; border-radius: 8px; box-shadow: 0 1px 3px rgba(0,0,0,0.1); }
        .card-value { font-size: 32px; font-weight: bold; color: #2563eb; margin-bottom: 5px; }
        .card-label { color: #666; font-size: 14px; }
        table { width: 100%; border-collapse: collapse; }
        th, td { text-align: left; padding: 6px 8px; border-bottom: 1px solid #eee; overflow-wrap: anywhere; }
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>RepoScope Analysis Report</h1>
            <div class="meta" id="repo-meta">Loading...</div>
        </div>
        <div class="cards" id="stats-cards"></div>
        <div class="section"><h2>Commits Over Time</h2><div id="commits-over-time"></div></div>
        <div class="section"><h2>Top Contributors</h2><div id="commits-by-author"></div></div>
        <div class="section"><h2>Activity by Directory</h2><div id="commits-by-directory"></div></div>
    </div>

    <script>
        // Every value from metrics.json (repository path, branch, author, file names) is repository-controlled:
        // only ever assign it through textContent.
        function add(parent, tag, text, cls) {
            const e = document.createElement(tag);
            if (text !== undefined) e.textContent = String(text);
            if (cls) e.className = cls;
            parent.appendChild(e);
            return e;
        }

        function table(id, heads, rows) {
            const host = document.getElementById(id);
            const t = add(host, 'table');
            const tr = add(add(t, 'thead'), 'tr');
            heads.forEach(h => add(tr, 'th', h));
            const body = add(t, 'tbody');
            rows.forEach(r => { const row = add(body, 'tr'); r.forEach(c => add(row, 'td', c)); });
        }

        function renderReport(m) {
            const meta = document.getElementById('repo-meta');
            meta.replaceChildren();
            [['Repository', m.repoPath], ['Branch', m.branch || 'N/A'], ['Head', String(m.headCommitId).substring(0, 8)]]
                .forEach(([k, v]) => { const d = add(meta, 'div'); add(d, 'strong', k + ': '); d.appendChild(document.createTextNode(String(v))); });

            const cards = document.getElementById('stats-cards');
            [['Total Commits', m.totalCommits], ['Contributors', m.uniqueAuthors], ['Directories', m.commitsByDirectory.length]]
                .forEach(([label, value]) => { const c = add(cards, 'div', undefined, 'card'); add(c, 'div', value, 'card-value'); add(c, 'div', label, 'card-label'); });

            table('commits-over-time', ['Day', 'Commits'], m.commitsOverTime.map(d => [d.day, d.commitCount]));
            table('commits-by-author', ['Author', 'Email', 'Commits'], m.commitsByAuthor.slice(0, 10).map(d => [d.authorName, d.authorEmail, d.commitCount]));
            table('commits-by-directory', ['Directory', 'Commits'], m.commitsByDirectory.slice(0, 10).map(d => [d.directoryPath, d.commitCount]));
        }

        fetch('metrics.json')
            .then(res => { if (!res.ok) throw new Error(res.statusText); return res.json(); })
            .then(renderReport)
            .catch(err => {
                console.error('Failed to load metrics:', err);
                document.getElementById('repo-meta').textContent = 'Failed to load metrics.json';
            });
    </script>
</body>
</html>
""";
    }
}
