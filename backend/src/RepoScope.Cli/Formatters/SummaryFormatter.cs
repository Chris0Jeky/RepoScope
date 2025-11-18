using System.Text;
using RepoScope.Core.Models;

namespace RepoScope.Cli.Formatters;

/// <summary>
/// Formats RepoMetrics as a human-readable summary.
/// </summary>
public static class SummaryFormatter
{
    public static string Format(RepoMetrics metrics)
    {
        var sb = new StringBuilder();

        sb.AppendLine("=== RepoScope Analysis ===");
        sb.AppendLine();
        sb.AppendLine($"Repository: {metrics.RepoPath}");
        sb.AppendLine($"Branch: {metrics.Branch ?? "N/A"}");
        sb.AppendLine($"Head Commit: {metrics.HeadCommitId[..Math.Min(8, metrics.HeadCommitId.Length)]}");
        sb.AppendLine();

        sb.AppendLine("--- Overview ---");
        sb.AppendLine($"Total Commits: {metrics.TotalCommits}");
        sb.AppendLine($"Unique Authors: {metrics.UniqueAuthors}");

        if (metrics.EarliestCommitDate.HasValue && metrics.LatestCommitDate.HasValue)
        {
            sb.AppendLine($"Date Range: {metrics.EarliestCommitDate.Value:yyyy-MM-dd} to {metrics.LatestCommitDate.Value:yyyy-MM-dd}");
        }

        sb.AppendLine();

        // Top authors
        if (metrics.CommitsByAuthor.Any())
        {
            sb.AppendLine("--- Top Authors ---");
            var topAuthors = metrics.CommitsByAuthor.Take(10);
            foreach (var author in topAuthors)
            {
                var percentage = (double)author.CommitCount / metrics.TotalCommits * 100;
                sb.AppendLine($"  {author.AuthorName} ({author.AuthorEmail}): {author.CommitCount} commits ({percentage:F1}%)");
            }
            sb.AppendLine();
        }

        // Most active directories
        if (metrics.CommitsByDirectory.Any())
        {
            sb.AppendLine("--- Most Active Directories ---");
            var topDirs = metrics.CommitsByDirectory.Take(10);
            foreach (var dir in topDirs)
            {
                sb.AppendLine($"  {dir.DirectoryPath}: {dir.CommitCount} commits");
            }
            sb.AppendLine();
        }

        // Most active days
        if (metrics.CommitsOverTime.Any())
        {
            sb.AppendLine("--- Most Active Days ---");
            var topDays = metrics.CommitsOverTime.OrderByDescending(d => d.CommitCount).Take(5);
            foreach (var day in topDays)
            {
                sb.AppendLine($"  {day.Day:yyyy-MM-dd}: {day.CommitCount} commits");
            }
        }

        return sb.ToString();
    }
}
