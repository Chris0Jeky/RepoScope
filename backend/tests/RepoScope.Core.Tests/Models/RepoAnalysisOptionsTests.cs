using RepoScope.Core.Models;
using Xunit;

namespace RepoScope.Core.Tests.Models;

public class RepoAnalysisOptionsTests
{
    [Fact]
    public void Default_ShouldHaveExpectedValues()
    {
        // Arrange & Act
        var options = RepoAnalysisOptions.Default;

        // Assert
        Assert.Equal("HEAD", options.Branch);
        Assert.Null(options.Since);
        Assert.Null(options.Until);
        Assert.Null(options.MaxCommits);
    }

    [Fact]
    public void InitSyntax_ShouldSetProperties()
    {
        // Arrange
        var since = DateTimeOffset.Now.AddDays(-30);
        var until = DateTimeOffset.Now;

        // Act
        var options = new RepoAnalysisOptions
        {
            Branch = "main",
            Since = since,
            Until = until,
            MaxCommits = 100
        };

        // Assert
        Assert.Equal("main", options.Branch);
        Assert.Equal(since, options.Since);
        Assert.Equal(until, options.Until);
        Assert.Equal(100, options.MaxCommits);
    }
}
