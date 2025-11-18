using System.Text.Json;
using System.Text.Json.Serialization;
using RepoScope.Core.Models;

namespace RepoScope.Cli.Formatters;

/// <summary>
/// Formats RepoMetrics as JSON.
/// </summary>
public static class JsonFormatter
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() }
    };

    public static string Format(RepoMetrics metrics)
    {
        return JsonSerializer.Serialize(metrics, Options);
    }

    public static void WriteToFile(RepoMetrics metrics, string filePath)
    {
        var json = Format(metrics);
        File.WriteAllText(filePath, json);
    }
}
