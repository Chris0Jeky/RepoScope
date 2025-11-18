namespace RepoScope.Core.Models;

/// <summary>
/// Options for controlling repository analysis behavior.
/// </summary>
public sealed class RepoAnalysisOptions
{
    /// <summary>
    /// Branch to analyze (default: "HEAD").
    /// </summary>
    public string Branch { get; init; } = "HEAD";

    /// <summary>
    /// Only include commits after this date (inclusive).
    /// </summary>
    public DateTimeOffset? Since { get; init; }

    /// <summary>
    /// Only include commits before this date (inclusive).
    /// </summary>
    public DateTimeOffset? Until { get; init; }

    /// <summary>
    /// Maximum number of commits to analyze.
    /// </summary>
    public int? MaxCommits { get; init; }

    /// <summary>
    /// Creates default options instance.
    /// </summary>
    public static RepoAnalysisOptions Default => new();
}
