using System.Text.RegularExpressions;
using RepoScope.Cli.Commands;
using Xunit;

namespace RepoScope.Core.Tests.Cli;

/// <summary>
/// Guards the generated report against #10 (markup from repository metadata) and #13 (remote executable code).
/// </summary>
public class ReportSafetyTests
{
    private static readonly string TemplateDir = FindTemplateDir();

    private static string FindTemplateDir()
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir != null; dir = dir.Parent)
        {
            var candidate = Path.Combine(dir.FullName, "templates", "report-template");
            if (Directory.Exists(candidate)) return candidate;
        }
        throw new DirectoryNotFoundException("templates/report-template not found above " + AppContext.BaseDirectory);
    }

    // Both the shipped template and the embedded fallback, as (name, html).
    public static IEnumerable<object[]> Pages()
    {
        yield return new object[] { "index.html", File.ReadAllText(Path.Combine(FindTemplateDir(), "index.html")) };
        yield return new object[] { "embedded fallback", ReportCommand.GetEmbeddedHtmlTemplate() };
    }

    [Theory]
    [MemberData(nameof(Pages))]
    public void Report_loads_no_remote_script_style_or_connection(string name, string html)
    {
        // #13: anything that starts with a scheme or // in src/href/@import/url() would leave the report folder.
        var remote = Regex.Matches(html, @"(?:src|href)\s*=\s*[""']\s*(?:[a-z][a-z0-9+.-]*:|//)|@import|url\(\s*[""']?\s*(?:[a-z][a-z0-9+.-]*:|//)", RegexOptions.IgnoreCase);
        Assert.True(remote.Count == 0, $"{name} references a remote resource: {string.Join(", ", remote.Select(m => m.Value))}");
        Assert.DoesNotContain("jsdelivr", html, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Content-Security-Policy", html);
        Assert.DoesNotContain("http:", html);
        Assert.DoesNotContain("https:", html);
    }

    [Fact]
    public void Template_ships_the_chart_library_it_references()
    {
        var html = File.ReadAllText(Path.Combine(TemplateDir, "index.html"));
        var srcs = Regex.Matches(html, @"<script[^>]+src=""([^""]+)""").Select(m => m.Groups[1].Value).ToList();
        Assert.Contains("assets/js/vendor/chart.umd.js", srcs);
        foreach (var src in srcs)
            Assert.True(File.Exists(Path.Combine(TemplateDir, src)), $"{src} is not shipped in the template");

        var chart = File.ReadAllText(Path.Combine(TemplateDir, "assets/js/vendor/chart.umd.js"));
        Assert.StartsWith("/*!", chart);
        Assert.Contains("Released under the MIT License", chart[..400]);
    }

    [Fact]
    public void Report_scripts_never_build_markup_from_strings()
    {
        // #10: repoPath/branch/authors/paths come from the analysed repository. Text must go in via
        // textContent; any HTML-string sink is a stored DOM injection.
        var scripts = new Dictionary<string, string>
        {
            ["assets/js/main.js"] = File.ReadAllText(Path.Combine(TemplateDir, "assets/js/main.js")),
            ["embedded fallback"] = ReportCommand.GetEmbeddedHtmlTemplate(),
        };
        var sinks = new[] { "innerHTML", "outerHTML", "insertAdjacentHTML", "document.write", "createContextualFragment", "DOMParser", "eval(", "new Function" };
        foreach (var (name, code) in scripts)
            foreach (var sink in sinks)
                Assert.False(code.Contains(sink, StringComparison.Ordinal), $"{name} uses {sink}");
    }
}
