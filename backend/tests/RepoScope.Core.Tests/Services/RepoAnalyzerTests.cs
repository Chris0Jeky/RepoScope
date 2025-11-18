using RepoScope.Core.Models;
using RepoScope.Core.Services;
using Xunit;

namespace RepoScope.Core.Tests.Services;

public class RepoAnalyzerTests
{
    [Fact]
    public void Analyze_WithNullPath_ThrowsArgumentException()
    {
        // Arrange
        var analyzer = new RepoAnalyzer();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => analyzer.Analyze(null!));
    }

    [Fact]
    public void Analyze_WithEmptyPath_ThrowsArgumentException()
    {
        // Arrange
        var analyzer = new RepoAnalyzer();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => analyzer.Analyze(string.Empty));
    }

    [Fact]
    public void Analyze_WithInvalidPath_ThrowsArgumentException()
    {
        // Arrange
        var analyzer = new RepoAnalyzer();
        var invalidPath = "/path/that/does/not/exist";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => analyzer.Analyze(invalidPath));
    }

    [Fact]
    public void Analyze_WithNonGitRepo_ThrowsArgumentException()
    {
        // Arrange
        var analyzer = new RepoAnalyzer();
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        try
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => analyzer.Analyze(tempDir));
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    // Note: More comprehensive tests would require setting up test repositories
    // or using mocking frameworks. For MVP, we focus on error handling tests.
}
