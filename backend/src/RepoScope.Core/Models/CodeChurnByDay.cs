namespace RepoScope.Core.Models;

/// <summary>
/// Represents code churn metrics (lines added/deleted) for a specific day.
/// </summary>
public sealed class CodeChurnByDay
{
    /// <summary>
    /// The date.
    /// </summary>
    public DateOnly Day { get; init; }

    /// <summary>
    /// Total lines added on this day.
    /// </summary>
    public int LinesAdded { get; init; }

    /// <summary>
    /// Total lines deleted on this day.
    /// </summary>
    public int LinesDeleted { get; init; }

    /// <summary>
    /// Net change (lines added - lines deleted) for this day.
    /// </summary>
    public int NetChange => LinesAdded - LinesDeleted;

    /// <summary>
    /// Total churn (lines added + lines deleted) for this day.
    /// </summary>
    public int TotalChurn => LinesAdded + LinesDeleted;

    /// <summary>
    /// Number of commits on this day.
    /// </summary>
    public int CommitCount { get; init; }
}
