using RepoScope.Core.Models;

namespace RepoScope.Core.Services;

/// <summary>
/// Service for analyzing Git repositories and computing metrics.
/// </summary>
public interface IRepoAnalyzer
{
    /// <summary>
    /// Analyzes a Git repository and returns computed metrics.
    /// </summary>
    /// <param name="repoPath">Absolute path to the Git repository.</param>
    /// <param name="options">Analysis options (optional).</param>
    /// <returns>Computed repository metrics.</returns>
    /// <exception cref="ArgumentException">If the path is not a valid Git repository.</exception>
    RepoMetrics Analyze(string repoPath, RepoAnalysisOptions? options = null);
}
