namespace RepoScope.Core.Models;

/// <summary>
/// Represents changes to a single file in a commit.
/// </summary>
public sealed class FileChangeInfo
{
    /// <summary>
    /// File path relative to repository root (e.g., "src/RepoScope/Core.cs").
    /// </summary>
    public string Path { get; init; } = default!;

    /// <summary>
    /// Number of lines added in this file.
    /// </summary>
    public int LinesAdded { get; init; }

    /// <summary>
    /// Number of lines deleted in this file.
    /// </summary>
    public int LinesDeleted { get; init; }
}
