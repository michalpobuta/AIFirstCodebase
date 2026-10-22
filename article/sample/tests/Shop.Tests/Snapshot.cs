using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Shop.Tests;

/// <summary>
/// Approval testing in a few lines. The expected output lives next to the test as
/// <c>&lt;Class&gt;.&lt;Test&gt;.verified.txt</c>; a mismatch writes <c>.received.txt</c> beside it.
/// Approving a change means a human replaces verified with received. An agent can't (see .claude/hooks).
/// </summary>
internal static partial class Snapshot
{
    public static void Matches(string actual, [CallerFilePath] string testFile = "", [CallerMemberName] string test = "")
    {
        var stem = Path.Combine(Path.GetDirectoryName(testFile)!, $"{Path.GetFileNameWithoutExtension(testFile)}.{test}");
        var verified = stem + ".verified.txt";
        var received = stem + ".received.txt";
        var scrubbed = ScrubGuids(actual);

        if (File.Exists(verified) && File.ReadAllText(verified) == scrubbed)
        {
            File.Delete(received);
            return;
        }

        File.WriteAllText(received, scrubbed);
        Assert.Fail($"Output differs from the approved snapshot. Compare {Path.GetFileName(received)} with " +
                    $"{Path.GetFileName(verified)}; if the change is intended, a human approves it by replacing the verified file.");
    }

    // Ids are random; what the snapshot pins is where they appear. Each distinct Guid becomes Guid_1, Guid_2, …
    private static string ScrubGuids(string text)
    {
        var seen = new Dictionary<string, string>();
        return GuidPattern().Replace(text, match =>
            seen.TryGetValue(match.Value, out var name) ? name : seen[match.Value] = $"Guid_{seen.Count + 1}");
    }

    [GeneratedRegex("[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}")]
    private static partial Regex GuidPattern();
}
