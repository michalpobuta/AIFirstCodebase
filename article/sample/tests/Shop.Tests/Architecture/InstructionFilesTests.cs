using System.Text.RegularExpressions;

namespace Shop.Tests.Architecture;

/// <summary>Every agent reads the same instructions, and there are few enough of them to be followed.</summary>
public sealed class InstructionFilesTests
{
    private const int MaxAgentsMdLines = 60;

    private static readonly string Root = FindRepositoryRoot();

    // Copilot Chat in Visual Studio reads .github/copilot-instructions.md and not AGENTS.md.
    [Fact]
    public void CopilotInstructions_AreAnExactCopyOfAgentsMd() => Assert.Equal(
        File.ReadAllText(Path.Combine(Root, "AGENTS.md")),
        File.ReadAllText(Path.Combine(Root, ".github", "copilot-instructions.md")));

    [Fact]
    public void AgentsMd_StaysWithinItsLineBudget()
    {
        var lines = File.ReadAllLines(Path.Combine(Root, "AGENTS.md")).Length;

        Assert.True(lines <= MaxAgentsMdLines,
            $"AGENTS.md has {lines} lines (budget {MaxAgentsMdLines}). Move rules the build can check into the build.");
    }

    // An instruction that points at code that's gone is an order the agent can't follow.
    [Fact]
    public void AgentsMd_PathsItNamesExist()
    {
        var files = Directory.EnumerateFiles(Root, "*", SearchOption.AllDirectories)
            .Select(file => Path.GetRelativePath(Root, file).Replace('\\', '/'))
            .Where(file => !file.Contains("/bin/", StringComparison.Ordinal) && !file.Contains("/obj/", StringComparison.Ordinal))
            .ToList();

        var missing = Regex.Matches(File.ReadAllText(Path.Combine(Root, "AGENTS.md")), @"`([\w.-]+/[\w./-]*)`")
            .Select(match => match.Groups[1].Value.TrimEnd('/'))
            .Where(path => !files.Exists(file => file.Contains(path, StringComparison.Ordinal)))
            .ToList();

        Assert.True(missing.Count == 0, "AGENTS.md names paths that don't exist: " + string.Join(", ", missing));
    }

    // "[test: X]" promises that a test named X enforces the rule. A tag without its test is just more prose.
    [Fact]
    public void AgentsMd_EnforcerTestsItNamesExist()
    {
        var tests = typeof(InstructionFilesTests).Assembly.GetTypes()
            .SelectMany(type => type.GetMethods())
            .Where(method => method.IsDefined(typeof(FactAttribute), inherit: false))
            .Select(method => method.Name)
            .ToHashSet();

        var named = Regex.Matches(File.ReadAllText(Path.Combine(Root, "AGENTS.md")), @"\[test: (\w+)\]")
            .Select(match => match.Groups[1].Value)
            .ToList();

        Assert.NotEmpty(named);
        Assert.True(named.TrueForAll(tests.Contains),
            "AGENTS.md names tests that don't exist: " + string.Join(", ", named.Where(name => !tests.Contains(name))));
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "AGENTS.md")))
            directory = directory.Parent;

        return directory?.FullName ?? throw new InvalidOperationException("No AGENTS.md above " + AppContext.BaseDirectory);
    }
}
