using System.Text;

namespace RepoScope.Cli.Formatters;

/// <summary>
/// Makes untrusted text (Git author names, branch names, paths, exception messages) safe to print to a
/// terminal. Control characters (C0, DEL, C1 - which includes ESC, so every CSI/OSC/hyperlink sequence,
/// plus CR/LF/tab), bidirectional controls and Unicode line separators are replaced with printable
/// \xNN / \uNNNN notation. Apply it at the presentation boundary only; JSON output keeps the raw data.
/// </summary>
public static class TerminalText
{
    public static string Escape(string? text)
    {
        if (string.IsNullOrEmpty(text)) return text ?? string.Empty;

        StringBuilder? sb = null;
        for (var i = 0; i < text.Length; i++)
        {
            var c = text[i];
            if (NeedsEscape(c))
            {
                sb ??= new StringBuilder(text.Length + 8).Append(text, 0, i);
                sb.Append(c <= 0xFF ? $"\\x{(int)c:X2}" : $"\\u{(int)c:X4}");
            }
            else
            {
                sb?.Append(c);
            }
        }

        return sb?.ToString() ?? text;
    }

    private static bool NeedsEscape(char c) =>
        char.IsControl(c)
        || c is '\u061C' or '\u200E' or '\u200F' or '\u2028' or '\u2029'
        || (c >= '\u202A' && c <= '\u202E')
        || (c >= '\u2066' && c <= '\u2069');
}
