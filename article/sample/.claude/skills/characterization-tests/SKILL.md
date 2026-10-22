---
name: characterization-tests
description: Pin what existing code does today before anyone changes it. Use before a refactor, when a hotspot has no tests, or when asked to test code you must not change yet.
---

# Characterization tests

The code already runs in production, so it is the answer key. You propose the inputs; running the code
records the outputs. You never decide what the right output is.

1. **Recon, read-only.** Don't edit any files. For the target, list its callers, hidden I/O (database, files,
   HTTP, clock, config), static state and candidate seams, with file:line for each.
2. **Write the tests, not the code.** Don't change production code. Put the inputs in one table, run them through
   the code and pin the output with `Snapshot.Matches` (`tests/Shop.Tests/Snapshot.cs`).
   Name the test `Method_currently_…`.
3. **Don't judge, list.** Expected something else? Don't change the test and don't fix the code. List it under
   "Suspicious" in your final message. A human decides: keep, bug, or delete.
4. **Leave the snapshot for a human.** The `.received.txt` stays for review; approving it is not your call.
5. **Prove it can fail.** Break the code once on purpose (flip a comparison), run the test, see it go red, revert.

Example: `tests/Shop.Tests/Features/Orders/DiscountPolicyCharacterizationTests.cs`.
