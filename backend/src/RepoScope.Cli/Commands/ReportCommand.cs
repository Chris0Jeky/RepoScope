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
        // For now, embed a simple HTML template
        // In production, this would copy from templates/report-template/
        var html = GetEmbeddedHtmlTemplate();
        await File.WriteAllTextAsync(outputPath, html);
    }

    private static string GetEmbeddedHtmlTemplate()
    {
        // Simple embedded template - will be replaced by external template later
        return @"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>RepoScope Report</title>
    <script src=""https://cdn.jsdelivr.net/npm/chart.js@4.4.1/dist/chart.umd.min.js""></script>
    <style>
        * { margin: 0; padding: 0; box-sizing: border-box; }
        body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; padding: 20px; background: #f5f5f5; }
        .container { max-width: 1200px; margin: 0 auto; }
        h1 { color: #333; margin-bottom: 10px; }
        .header { background: white; padding: 30px; border-radius: 8px; margin-bottom: 20px; box-shadow: 0 1px 3px rgba(0,0,0,0.1); }
        .meta { color: #666; font-size: 14px; }
        .cards { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 20px; margin-bottom: 20px; }
        .card { background: white; padding: 20px; border-radius: 8px; box-shadow: 0 1px 3px rgba(0,0,0,0.1); }
        .card-value { font-size: 32px; font-weight: bold; color: #2563eb; margin-bottom: 5px; }
        .card-label { color: #666; font-size: 14px; }
        .chart-container { background: white; padding: 30px; border-radius: 8px; margin-bottom: 20px; box-shadow: 0 1px 3px rgba(0,0,0,0.1); }
        canvas { max-height: 400px; }
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>RepoScope Analysis Report</h1>
            <div class=""meta"" id=""repo-meta"">Loading...</div>
        </div>

        <div class=""cards"" id=""stats-cards""></div>

        <div class=""chart-container"">
            <h2>Commits Over Time</h2>
            <canvas id=""commits-over-time""></canvas>
        </div>

        <div class=""chart-container"">
            <h2>Top Contributors</h2>
            <canvas id=""commits-by-author""></canvas>
        </div>

        <div class=""chart-container"">
            <h2>Activity by Directory</h2>
            <canvas id=""commits-by-directory""></canvas>
        </div>
    </div>

    <script>
        // Load metrics
        fetch('metrics.json')
            .then(res => res.json())
            .then(data => {
                renderReport(data);
            })
            .catch(err => {
                console.error('Failed to load metrics:', err);
                document.querySelector('.container').innerHTML = '<p>Failed to load metrics.json</p>';
            });

        function renderReport(metrics) {
            // Header
            document.getElementById('repo-meta').innerHTML = `
                <strong>Repository:</strong> ${metrics.repoPath}<br>
                <strong>Branch:</strong> ${metrics.branch || 'N/A'}<br>
                <strong>Head:</strong> ${metrics.headCommitId.substring(0, 8)}
            `;

            // Stats cards
            const cardsHtml = `
                <div class=""card"">
                    <div class=""card-value"">${metrics.totalCommits}</div>
                    <div class=""card-label"">Total Commits</div>
                </div>
                <div class=""card"">
                    <div class=""card-value"">${metrics.uniqueAuthors}</div>
                    <div class=""card-label"">Contributors</div>
                </div>
                <div class=""card"">
                    <div class=""card-value"">${metrics.commitsByDirectory.length}</div>
                    <div class=""card-label"">Directories</div>
                </div>
            `;
            document.getElementById('stats-cards').innerHTML = cardsHtml;

            // Charts
            renderCommitsOverTime(metrics.commitsOverTime);
            renderCommitsByAuthor(metrics.commitsByAuthor);
            renderCommitsByDirectory(metrics.commitsByDirectory);
        }

        function renderCommitsOverTime(data) {
            const ctx = document.getElementById('commits-over-time');
            new Chart(ctx, {
                type: 'line',
                data: {
                    labels: data.map(d => d.day),
                    datasets: [{
                        label: 'Commits',
                        data: data.map(d => d.commitCount),
                        borderColor: '#2563eb',
                        backgroundColor: 'rgba(37, 99, 235, 0.1)',
                        tension: 0.1,
                        fill: true
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: true,
                    plugins: { legend: { display: false } }
                }
            });
        }

        function renderCommitsByAuthor(data) {
            const top10 = data.slice(0, 10);
            const ctx = document.getElementById('commits-by-author');
            new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: top10.map(d => d.authorName),
                    datasets: [{
                        label: 'Commits',
                        data: top10.map(d => d.commitCount),
                        backgroundColor: '#2563eb'
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: true,
                    indexAxis: 'y',
                    plugins: { legend: { display: false } }
                }
            });
        }

        function renderCommitsByDirectory(data) {
            const top10 = data.slice(0, 10);
            const ctx = document.getElementById('commits-by-directory');
            new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: top10.map(d => d.directoryPath),
                    datasets: [{
                        label: 'Commits',
                        data: top10.map(d => d.commitCount),
                        backgroundColor: '#10b981'
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: true,
                    plugins: { legend: { display: false } }
                }
            });
        }
    </script>
</body>
</html>";
    }
}
