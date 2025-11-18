namespace RepoScope.Core.Models;

/// <summary>
/// Aggregated commit count for a specific directory.
/// </summary>
public sealed class CommitsByDirectory
{
    /// <summary>
    /// Directory path relative to repository root (e.g., "src", "src/Core").
    /// </summary>
    public string DirectoryPath { get; init; } = default!;

    /// <summary>
    /// Number of commits that touched files in this directory.
    /// </summary>
    public int CommitCount { get; init; }
}
