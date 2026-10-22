# Shop — sample for “AI-First Codebase”

The companion project for the article [AI-First Codebase: How to Set Up Your .NET Project for Working with AI](../ai-first-codebase.md).
A small order-management HTTP API (create, pay, ship, a CSV export for the warehouse) on .NET 10 with minimal APIs.
The app is deliberately boring; what matters is everything around it.

```bash
dotnet build
dotnet test        # 25 tests
```

Needs the .NET 10 SDK. `global.json` opts into Microsoft.Testing.Platform, so a run with zero tests fails.

## What's where

| Question the agent asks | What answers it | Files |
|---|---|---|
| Where does this go? | One use case per file, wiring listed by hand, structure as tests | `src/Shop/Features/`, `*Module.cs`, `tests/Shop.Tests/Architecture/ArchitectureTests.cs`, `eng/hotspots.sh` |
| What are the rules? | The build: nullable, warnings as errors, analyzers, banned APIs, an enum switch without `_ =>` | `Directory.Build.props`, `BannedSymbols.txt`, `.editorconfig`, `src/Shop/Features/Warehouse/WarehouseExport.cs` |
| How do I know I'm done? | One command, zero tests = red, a minimum test count in CI, checks the agent can't change | `global.json`, `.github/workflows/`, `.github/CODEOWNERS`, `eng/check-suppressions.sh`, `.claude/` |
| What must I not break? | A short root file, a tested copy for Visual Studio, context on demand | `AGENTS.md`, `CLAUDE.md`, `.github/copilot-instructions.md`, `.github/instructions/`, `.claude/skills/`, `tests/Shop.Tests/Architecture/InstructionFilesTests.cs` |
| Old code you can't change yet | A characterization test: inputs proposed, outputs recorded by running the code, sorted by a human | `tests/Shop.Tests/Features/Orders/DiscountPolicyCharacterizationTests.cs` and its `.verified.txt` |

## Before you copy it

- **CODEOWNERS** uses placeholder teams (`@your-org/leads`). It enforces nothing until you turn on
  “Require review from Code Owners” in the branch rules.
- **Hooks** in `.claude/` are for Claude Code. They are C# file-based apps (`dotnet run --file`), so the first
  call compiles them. They guard file edits, not the shell; CI is the backstop. `protect-tests.cs` treats only
  files tracked by git as existing tests.
- **`eng/*.sh`** need bash (on Windows: Git Bash). `check-suppressions.sh` needs the base branch fetched
  (`fetch-depth: 0`); without it, the step fails instead of passing.
- **`--minimum-expected-tests`** in `ci.yml` is a Microsoft.Testing.Platform option. Raise it when you add tests.
- **The approved snapshots** (`*.verified.txt`) are contracts. A mismatch writes `*.received.txt`; a human
  approves a change by replacing the verified file.
