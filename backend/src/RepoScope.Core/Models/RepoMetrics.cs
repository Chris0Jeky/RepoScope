namespace RepoScope.Core.Models;

/// <summary>
/// Comprehensive metrics computed from repository analysis.
/// </summary>
public sealed class RepoMetrics
{
    /// <summary>
    /// Absolute path to the analyzed repository.
    /// </summary>
    public string RepoPath { get; init; } = default!;

    /// <summary>
    /// Branch that was analyzed.
    /// </summary>
    public string? Branch { get; init; }

    /// <summary>
    /// SHA of the HEAD commit at analysis time.
    /// </summary>
    public string HeadCommitId { get; init; } = default!;

    /// <summary>
    /// Total number of commits analyzed.
    /// </summary>
    public int TotalCommits { get; init; }

    /// <summary>
    /// Date range of analyzed commits.
    /// </summary>
    public DateTimeOffset? EarliestCommitDate { get; init; }

    /// <summary>
    /// Date of the most recent commit.
    /// </summary>
    public DateTimeOffset? LatestCommitDate { get; init; }

    /// <summary>
    /// Number of unique authors.
    /// </summary>
    public int UniqueAuthors { get; init; }

    /// <summary>
    /// Commits grouped by day.
    /// </summary>
    public IReadOnlyList<CommitsByDay> CommitsOverTime { get; init; } = Array.Empty<CommitsByDay>();

    /// <summary>
    /// Commits grouped by author.
    /// </summary>
    public IReadOnlyList<CommitsByAuthor> CommitsByAuthor { get; init; } = Array.Empty<CommitsByAuthor>();

    /// <summary>
    /// Commits grouped by directory.
    /// </summary>
    public IReadOnlyList<CommitsByDirectory> CommitsByDirectory { get; init; } = Array.Empty<CommitsByDirectory>();
}
