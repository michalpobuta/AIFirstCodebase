# Shop

Order-management HTTP API. .NET 10, minimal APIs, in-memory storage.

## Build and test

- `dotnet build`, then `dotnet test`. A run with zero tests fails (Microsoft.Testing.Platform, set in global.json).
- Warnings are errors. The analyzers and the architecture tests are the conventions; there is no style guide to read.

## Enforced by the build, not repeated here

- Style, nullability, analyzers              [enforced: dotnet build · Directory.Build.props]
- No wall clock in app code                  [enforced: RS0030 · BannedSymbols.txt]
- OrderStatus switches have no `_` arm       [enforced: CS8509 · TreatWarningsAsErrors]
- No *Manager, *Helper or *Utils types       [test: TypeNames_SayWhatTheTypeDoes]
- Handlers don't reference ASP.NET Core      [test: Handlers_DoNotKnowAboutHttp]

## Where code goes

- One use case = one file in `src/Shop/Features/<Area>/`: request record, handler, endpoint.
- Register handlers and map endpoints by hand in `<Area>Module.cs`. No assembly scanning.
- Tests mirror the source path under `tests/Shop.Tests/Features/`.

## The specification

- Existing tests are the specification. Add tests freely; never edit or delete one to make code pass.
- Adding behaviour: write the failing test first, named `Method_Scenario_ExpectedResult`.
- Before you claim a fix works, show the test failing without it.
- If the specification is unclear or seems wrong (a test, or a contract like the warehouse CSV), stop and
  begin your final message with `SPEC QUESTION:`. That is the only way to finish with a red build.

## Things the code can't tell you

- The warehouse CSV (`src/Shop/Features/Warehouse/`) is parsed by the warehouse's own system. Existing
  status codes never change; a new code needs a human-approved snapshot.   [review: CODEOWNERS]
- Money is `decimal`, rounded once, at the end, half away from zero: finance reconciles against
  invoices rounded that way.

## Where to look next

- Test conventions load with the test files: `.github/instructions/tests.instructions.md`.
- Warehouse contract details load with the warehouse code: `.github/instructions/warehouse.instructions.md`.
- Tests for code you must not change yet: `.claude/skills/characterization-tests/SKILL.md`.
