# The AI-First .NET Codebase

A playbook for setting up a .NET repository so that a coding agent can do the work without you stepping in,
and a sample project that implements every practice in it.

- **Read:** [The AI-First .NET Codebase: A Playbook](article/ai-first-codebase.md)
- **Try:** [the sample project](article/sample/), a small order API on .NET 10. The playbook links to its exact lines.

```bash
cd article/sample
dotnet test
```

Needs the .NET 10 SDK.

## The idea in one paragraph

Agents fail on a codebase for two reasons: structure they can't navigate, and knowledge they can't infer.
Instruction files help a little: advice lowers the rate of mistakes. What holds regardless of the model is
everything outside the agent's reach that can say no (the build, CI, a required review): the build sets the
floor. So encode each rule as high up as it goes, from types and the compiler to tests and CI gates, and keep
prose for what the code can't say.

## What's in the playbook

| Part | What you get |
|---|---|
| [The approach](article/ai-first-codebase.md#the-approach) | What "AI-first" means, how to measure it, and why the build comes before instruction files |
| [Layer 1 · Structure](article/ai-first-codebase.md#layer-1--structure-the-agent-can-navigate) | Use-case names, traceable wiring, architecture tests, where to refactor first |
| [Layer 2 · Rules](article/ai-first-codebase.md#layer-2--rules-the-build-enforces) | `Directory.Build.props`, exhaustive enum switches, banned APIs, suppressions behind review |
| [Layer 3 · Checks](article/ai-first-codebase.md#layer-3--checks-the-agent-cant-fake) | One test command, zero tests = red, tests that can fail, CODEOWNERS, hooks |
| [Layer 4 · Knowledge](article/ai-first-codebase.md#layer-4--knowledge-the-code-cant-give) | A short `AGENTS.md`, a tested copy for Visual Studio, path-scoped notes and skills |
| [Playbook A · New repository](article/ai-first-codebase.md#playbook-a--a-new-repository) | Day one in about an hour |
| [Playbook B · Existing repository](article/ai-first-codebase.md#playbook-b--an-existing-repository) | Make it run, pin it with characterization tests, write down what's hidden, then change it |
| [Your first week](article/ai-first-codebase.md#your-first-week) | Five changes, one per day |

Evidence is as of October 2026. Every number names its source, its evidence type and the model generation it
was measured on.
