using System.CommandLine;
using RepoScope.Cli.Formatters;
using RepoScope.Core.Models;
using RepoScope.Core.Services;

namespace RepoScope.Cli.Commands;

/// <summary>
/// Command to analyze a repository and output JSON metrics.
/// </summary>
public static class AnalyzeCommand
{
    public static Command Create()
    {
        var command = new Command("analyze", "Analyze a Git repository and output metrics as JSON");

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

        var outputOpt = new Option<string?>(
            "--out",
            description: "Output file path (default: stdout)");

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

                Console.Error.WriteLine($"Analyzing repository at {path}...");
                var metrics = analyzer.Analyze(path, options);

                var json = JsonFormatter.Format(metrics);

                if (output != null)
                {
                    await File.WriteAllTextAsync(output, json);
                    Console.Error.WriteLine($"Metrics written to {output}");
                }
                else
                {
                    Console.WriteLine(json);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                Environment.Exit(1);
            }
        }, pathArg, branchOpt, sinceOpt, untilOpt, maxCommitsOpt, outputOpt);

        return command;
    }
}
