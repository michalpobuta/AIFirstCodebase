// PreToolUse hook (Edit|Write): existing tests are the specification.
// The agent may add test files; it may not rewrite the ones that pin today's behaviour,
// and it never writes an approved snapshot (*.verified.*) — approving is a human decision.
// Exit code 2 blocks the edit, and stderr goes back to the agent as the reason.
using System.Diagnostics;
using System.Text.Json;

using var input = JsonDocument.Parse(Console.In.ReadToEnd());
if (!input.RootElement.TryGetProperty("tool_input", out var tool) ||
    !tool.TryGetProperty("file_path", out var filePath))
    return 0;

var root = Environment.GetEnvironmentVariable("CLAUDE_PROJECT_DIR") ?? Directory.GetCurrentDirectory();
var path = Path.GetRelativePath(root, filePath.GetString()!).Replace('\\', '/');

if (path.Contains(".verified.", StringComparison.Ordinal))
{
    Console.Error.WriteLine($"""
        {path} is an approved snapshot. Only a human approves snapshots.
        Leave the .received file for review and begin your final message with "SPEC QUESTION:" explaining the change.
        """);
    return 2;
}

// TestShop.cs wires the app up for tests — new use cases extend it. Everything else under tests/ is specification.
const string TestWiring = "tests/Shop.Tests/TestShop.cs";
if (!path.StartsWith("tests/", StringComparison.Ordinal) || path == TestWiring || !IsTracked(root, path))
    return 0;

Console.Error.WriteLine($"""
    {path} is an existing test, and existing tests are the specification.
    Change the code so the test passes, or add a new test file.
    If you believe this test is wrong, stop and begin your final message with "SPEC QUESTION:" and say why.
    """);
return 2;

static bool IsTracked(string root, string path)
{
    using var git = Process.Start(new ProcessStartInfo("git", ["ls-files", "--error-unmatch", "--", path])
    {
        WorkingDirectory = root,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
    })!;
    git.WaitForExit();
    return git.ExitCode == 0;
}
