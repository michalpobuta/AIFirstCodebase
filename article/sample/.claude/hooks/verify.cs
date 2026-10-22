// Stop hook: the agent can't finish while the build or the tests are red —
// unless it raises a spec question, which hands the decision to a human.
// Exit code 2 keeps the agent working, with stderr as the reason. Only the failures go back:
// a full build and test log costs thousands of tokens, the failures cost a few dozen.
using System.Diagnostics;
using System.Text.Json;

using var input = JsonDocument.Parse(Console.In.ReadToEnd());
var lastMessage = input.RootElement.TryGetProperty("last_assistant_message", out var message)
    ? message.GetString() ?? ""
    : "";
// Must *begin* with the marker. An agent once wrote "This isn't a SPEC QUESTION:" and a Contains() check let it out.
if (lastMessage.TrimStart().StartsWith("SPEC QUESTION:", StringComparison.Ordinal))
    return 0;

var root = Environment.GetEnvironmentVariable("CLAUDE_PROJECT_DIR") ?? Directory.GetCurrentDirectory();
if (Run("git", "status", "--porcelain", "--", "src", "tests", "Directory.Build.props", "Directory.Packages.props", ".editorconfig", "global.json", "BannedSymbols.txt").Output.Length == 0)
    return 0; // nothing changed, nothing to verify

var build = Run("dotnet", "build", "--nologo", "-v", "q", "-clp:NoSummary");
if (build.ExitCode != 0)
    return Block("The build fails:", build.Output.Split('\n').Where(line => line.Contains(": error ", StringComparison.Ordinal)).Distinct());

var test = Run("dotnet", "test", "--no-build", "--no-progress", "--no-ansi");
if (test.ExitCode != 0)
    return Block("Tests fail:", Failures(test.Output));

return 0;

int Block(string headline, IEnumerable<string> details)
{
    Console.Error.WriteLine(headline);
    foreach (var line in details.Take(40))
        Console.Error.WriteLine(line.Replace(root + Path.DirectorySeparatorChar, "", StringComparison.Ordinal));
    Console.Error.WriteLine("Fix the code, not the tests. If a test is wrong, stop with SPEC QUESTION: and say why.");
    return 2;
}

// From each "failed <test>" block keep the name and the assertion message; drop stack frames.
static IEnumerable<string> Failures(string output)
{
    var lines = output.Split('\n').Select(line => line.TrimEnd());
    var inFailure = false;
    var any = false;
    foreach (var line in lines)
    {
        if (line.StartsWith("failed ", StringComparison.Ordinal)) { inFailure = any = true; yield return line; continue; }
        if (!inFailure || line.StartsWith("  from ", StringComparison.Ordinal)) continue;
        if (line.StartsWith("    at ", StringComparison.Ordinal) || line.Length == 0) { inFailure = false; continue; }
        yield return line;
    }
    if (!any) // e.g. zero tests ran: the summary is the message
        foreach (var line in lines.Where(line => line.Contains("Zero tests ran", StringComparison.Ordinal) || line.Contains("exit code", StringComparison.Ordinal)))
            yield return line;
}

(int ExitCode, string Output) Run(string file, params string[] args)
{
    var start = new ProcessStartInfo(file, args)
    {
        WorkingDirectory = root,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
    };
    start.Environment["DOTNET_CLI_UI_LANGUAGE"] = "en";
    using var process = Process.Start(start)!;
    var stdout = process.StandardOutput.ReadToEndAsync();
    var stderr = process.StandardError.ReadToEnd();
    process.WaitForExit();
    return (process.ExitCode, stdout.Result + stderr);
}
