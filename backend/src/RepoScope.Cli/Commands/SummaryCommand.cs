using System.CommandLine;
using RepoScope.Cli.Formatters;
using RepoScope.Core.Models;
using RepoScope.Core.Services;

namespace RepoScope.Cli.Commands;

/// <summary>
/// Command to analyze a repository and print a human-readable summary.
/// </summary>
public static class SummaryCommand
{
    public static Command Create()
    {
        var command = new Command("summary", "Analyze a Git repository and print a summary");

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

        command.AddArgument(pathArg);
        command.AddOption(branchOpt);
        command.AddOption(sinceOpt);
        command.AddOption(untilOpt);
        command.AddOption(maxCommitsOpt);

        command.SetHandler((path, branch, since, until, maxCommits) =>
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

                var metrics = analyzer.Analyze(path, options);
                var summary = SummaryFormatter.Format(metrics);

                Console.WriteLine(summary);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                Environment.Exit(1);
            }
        }, pathArg, branchOpt, sinceOpt, untilOpt, maxCommitsOpt);

        return command;
    }
}
