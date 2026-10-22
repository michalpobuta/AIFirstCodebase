---
applyTo: "tests/**"
---
# Tests

- Build the app through `tests/Shop.Tests/TestShop.cs`: it wires the shop the way `Program.cs` does, with a
  `FakeTimeProvider` the test advances. Never wait on real time.
- Names: `Method_Scenario_ExpectedResult`. Characterization tests are `Method_currently_…`: they record what
  the code does today, not what it should do.
- Snapshots: a mismatch writes `*.received.txt` next to the test. Only a human replaces `*.verified.txt`.
