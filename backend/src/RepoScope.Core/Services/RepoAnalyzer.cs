using LibGit2Sharp;
using RepoScope.Core.Models;

namespace RepoScope.Core.Services;

/// <summary>
/// Default implementation of repository analyzer using LibGit2Sharp.
/// </summary>
public sealed class RepoAnalyzer : IRepoAnalyzer
{
    public RepoMetrics Analyze(string repoPath, RepoAnalysisOptions? options = null)
    {
        options ??= RepoAnalysisOptions.Default;

        if (string.IsNullOrWhiteSpace(repoPath))
        {
            throw new ArgumentException("Repository path cannot be empty.", nameof(repoPath));
        }

        if (!Repository.IsValid(repoPath))
        {
            throw new ArgumentException($"Path '{repoPath}' is not a valid Git repository.", nameof(repoPath));
        }

        using var repo = new Repository(repoPath);

        // Get the branch to analyze
        var branch = GetBranch(repo, options.Branch);
        var commits = ExtractCommits(repo, branch, options);

        // Aggregate metrics
        return AggregateMetrics(repoPath, options.Branch, repo.Head.Tip.Sha, commits);
    }

    private static Branch GetBranch(Repository repo, string branchName)
    {
        if (branchName == "HEAD")
        {
            return repo.Head;
        }

        var branch = repo.Branches[branchName];
        if (branch == null)
        {
            throw new ArgumentException($"Branch '{branchName}' not found in repository.", nameof(branchName));
        }

        return branch;
    }

    private static List<CommitInfo> ExtractCommits(Repository repo, Branch branch, RepoAnalysisOptions options)
    {
        var commits = new List<CommitInfo>();
        var filter = new CommitFilter
        {
            IncludeReachableFrom = branch,
            SortBy = CommitSortStrategies.Time | CommitSortStrategies.Reverse
        };

        foreach (var commit in repo.Commits.QueryBy(filter))
        {
            // Apply date filters
            if (options.Since.HasValue && commit.Author.When < options.Since.Value)
            {
                continue;
            }

            if (options.Until.HasValue && commit.Author.When > options.Until.Value)
            {
                continue;
            }

            // Apply max commits limit
            if (options.MaxCommits.HasValue && commits.Count >= options.MaxCommits.Value)
            {
                break;
            }

            var commitInfo = new CommitInfo
            {
                Id = commit.Sha,
                AuthorName = commit.Author.Name,
                AuthorEmail = commit.Author.Email,
                AuthorDate = commit.Author.When,
                Message = commit.Message,
                FileChanges = ExtractFileChanges(repo, commit)
            };

            commits.Add(commitInfo);
        }

        return commits;
    }

    private static List<FileChangeInfo> ExtractFileChanges(Repository repo, Commit commit)
    {
        var fileChanges = new List<FileChangeInfo>();

        // Compare with first parent (or empty tree if this is the first commit)
        var parent = commit.Parents.FirstOrDefault();
        var changes = parent != null
            ? repo.Diff.Compare<TreeChanges>(parent.Tree, commit.Tree)
            : repo.Diff.Compare<TreeChanges>(null, commit.Tree);

        foreach (var change in changes)
        {
            var patch = repo.Diff.Compare<Patch>(parent?.Tree, commit.Tree);
            var filePatch = patch[change.Path];

            var fileChange = new FileChangeInfo
            {
                Path = change.Path,
                LinesAdded = filePatch?.LinesAdded ?? 0,
                LinesDeleted = filePatch?.LinesDeleted ?? 0
            };

            fileChanges.Add(fileChange);
        }

        return fileChanges;
    }

    private static RepoMetrics AggregateMetrics(string repoPath, string branch, string headCommitId, List<CommitInfo> commits)
    {
        if (commits.Count == 0)
        {
            return new RepoMetrics
            {
                RepoPath = repoPath,
                Branch = branch,
                HeadCommitId = headCommitId,
                TotalCommits = 0,
                UniqueAuthors = 0
            };
        }

        // Commits over time
        var commitsByDay = commits
            .GroupBy(c => DateOnly.FromDateTime(c.AuthorDate.Date))
            .Select(g => new CommitsByDay
            {
                Day = g.Key,
                CommitCount = g.Count()
            })
            .OrderBy(c => c.Day)
            .ToList();

        // Commits by author
        var commitsByAuthor = commits
            .GroupBy(c => new { c.AuthorName, c.AuthorEmail })
            .Select(g => new CommitsByAuthor
            {
                AuthorName = g.Key.AuthorName,
                AuthorEmail = g.Key.AuthorEmail,
                CommitCount = g.Count()
            })
            .OrderByDescending(a => a.CommitCount)
            .ToList();

        // Commits by directory
        var commitsByDirectory = commits
            .SelectMany(c => c.FileChanges.Select(fc => new { Commit = c, FileChange = fc }))
            .GroupBy(x => GetTopLevelDirectory(x.FileChange.Path))
            .Select(g => new CommitsByDirectory
            {
                DirectoryPath = g.Key,
                CommitCount = g.Select(x => x.Commit.Id).Distinct().Count()
            })
            .OrderByDescending(d => d.CommitCount)
            .ToList();

        return new RepoMetrics
        {
            RepoPath = repoPath,
            Branch = branch,
            HeadCommitId = headCommitId,
            TotalCommits = commits.Count,
            EarliestCommitDate = commits.Min(c => c.AuthorDate),
            LatestCommitDate = commits.Max(c => c.AuthorDate),
            UniqueAuthors = commitsByAuthor.Count,
            CommitsOverTime = commitsByDay,
            CommitsByAuthor = commitsByAuthor,
            CommitsByDirectory = commitsByDirectory
        };
    }

    private static string GetTopLevelDirectory(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return "(root)";
        }

        var segments = filePath.Split('/', '\\');
        return segments.Length > 1 ? segments[0] : "(root)";
    }
}
