namespace RepoScope.Core.Models;

/// <summary>
/// Represents a single Git commit with its metadata and file changes.
/// </summary>
public sealed class CommitInfo
{
    /// <summary>
    /// Commit SHA identifier.
    /// </summary>
    public string Id { get; init; } = default!;

    /// <summary>
    /// Author name.
    /// </summary>
    public string AuthorName { get; init; } = default!;

    /// <summary>
    /// Author email address.
    /// </summary>
    public string AuthorEmail { get; init; } = default!;

    /// <summary>
    /// Date and time when the commit was authored.
    /// </summary>
    public DateTimeOffset AuthorDate { get; init; }

    /// <summary>
    /// Commit message.
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// List of file changes in this commit.
    /// </summary>
    public IReadOnlyList<FileChangeInfo> FileChanges { get; init; } = Array.Empty<FileChangeInfo>();
}
