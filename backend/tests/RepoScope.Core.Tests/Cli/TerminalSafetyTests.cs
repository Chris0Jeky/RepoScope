using RepoScope.Cli.Formatters;
using RepoScope.Core.Models;
using Xunit;

namespace RepoScope.Core.Tests.Cli;

/// <summary>Guards the CLI summary against #14 (terminal control sequences from Git metadata).</summary>
public class TerminalSafetyTests
{
    private static RepoMetrics Metrics(string name, string email, string path = "/repo", string? branch = "main", string dir = "src") => new()
    {
        RepoPath = path,
        Branch = branch,
        HeadCommitId = "0123456789abcdef",
        TotalCommits = 1,
        UniqueAuthors = 1,
        CommitsByAuthor = new[] { new CommitsByAuthor { AuthorName = name, AuthorEmail = email, CommitCount = 1 } },
        CommitsByDirectory = new[] { new CommitsByDirectory { DirectoryPath = dir, CommitCount = 1 } },
    };

    private static bool HasUnsafeChar(string s) =>
        s.Replace(Environment.NewLine, "").Any(c => char.IsControl(c) || (c >= '\u202A' && c <= '\u202E') || (c >= '\u2066' && c <= '\u2069') || c is '\u2028' or '\u2029');

    [Theory]
    [InlineData("\u001b[2J\u001b]0;pwned\u0007\u001b]8;;http://x\u001b\\link", "evil", "name")]
    [InlineData("a\nSpoofed: 100% clean\r", "e@x.y", "name")]
    [InlineData("n", "\u009b31m\u0085@x.y", "name")]
    [InlineData("\u202Eevil", "e@x.y", "name")]
    [InlineData("n", "e@x.y", "dir\u001b[31m")]
    public void Summary_prints_no_control_or_bidi_characters_from_git_metadata(string name, string email, string dir)
    {
        var text = SummaryFormatter.Format(Metrics(name, email, "/r\u001b[1mepo", "br\u001b[2Janch", dir));

        Assert.False(HasUnsafeChar(text), "summary contains raw control/bidi characters");
        Assert.DoesNotContain('\u001b', text);
        // a forged newline must not add output lines
        Assert.Equal(SummaryFormatter.Format(Metrics("n", "e")).Split(Environment.NewLine).Length, text.Split(Environment.NewLine).Length);
    }

    [Fact]
    public void Summary_shows_escapes_visibly_and_keeps_ordinary_unicode()
    {
        var text = SummaryFormatter.Format(Metrics("\u001b[2Jx", "jos\u00e9@example.com"));
        Assert.Contains(@"\x1B[2Jx", text);

        var benign = SummaryFormatter.Format(Metrics("Zo\u00eb \u5f20\u4f1f \U0001F468\u200D\U0001F4BB", "a@b.c", "/home/Jos\u00e9/repo with space"));
        Assert.Contains("Zo\u00eb \u5f20\u4f1f \U0001F468\u200D\U0001F4BB (a@b.c)", benign);
        Assert.Contains("/home/Jos\u00e9/repo with space", benign);
    }
}
