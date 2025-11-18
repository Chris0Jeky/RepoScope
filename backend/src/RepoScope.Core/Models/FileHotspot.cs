namespace RepoScope.Core.Models;

/// <summary>
/// Represents file-level activity metrics showing which files have the most commits.
/// </summary>
public sealed class FileHotspot
{
    /// <summary>
    /// Full path to the file relative to repository root.
    /// </summary>
    public string FilePath { get; init; } = default!;

    /// <summary>
    /// Number of commits that touched this file.
    /// </summary>
    public int CommitCount { get; init; }

    /// <summary>
    /// Total lines added across all commits for this file.
    /// </summary>
    public int LinesAdded { get; init; }

    /// <summary>
    /// Total lines deleted across all commits for this file.
    /// </summary>
    public int LinesDeleted { get; init; }

    /// <summary>
    /// Net change in lines (added - deleted).
    /// </summary>
    public int NetChange => LinesAdded - LinesDeleted;

    /// <summary>
    /// Total churn (lines added + lines deleted) - indicates file volatility.
    /// </summary>
    public int TotalChurn => LinesAdded + LinesDeleted;
}
