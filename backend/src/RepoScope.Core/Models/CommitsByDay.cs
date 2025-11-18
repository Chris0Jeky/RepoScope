namespace RepoScope.Core.Models;

/// <summary>
/// Aggregated commit count for a specific day.
/// </summary>
public sealed class CommitsByDay
{
    /// <summary>
    /// The date.
    /// </summary>
    public DateOnly Day { get; init; }

    /// <summary>
    /// Number of commits on this day.
    /// </summary>
    public int CommitCount { get; init; }
}
