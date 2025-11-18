namespace RepoScope.Core.Models;

/// <summary>
/// Aggregated commit count for a specific author.
/// </summary>
public sealed class CommitsByAuthor
{
    /// <summary>
    /// Author name.
    /// </summary>
    public string AuthorName { get; init; } = default!;

    /// <summary>
    /// Author email address.
    /// </summary>
    public string AuthorEmail { get; init; } = default!;

    /// <summary>
    /// Total number of commits by this author.
    /// </summary>
    public int CommitCount { get; init; }
}
