using System.CommandLine;
using RepoScope.Cli.Commands;

namespace RepoScope.Cli;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("RepoScope - Git repository analyzer and visualizer");

        rootCommand.AddCommand(AnalyzeCommand.Create());
        rootCommand.AddCommand(SummaryCommand.Create());
        rootCommand.AddCommand(ReportCommand.Create());

        return await rootCommand.InvokeAsync(args);
    }
}
