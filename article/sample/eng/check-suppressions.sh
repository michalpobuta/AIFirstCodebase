#!/usr/bin/env bash
# The fastest way to green is switching the rule off. This makes that a reviewer's decision, not a green build:
# fails on an added suppression or skip, or a removed test, between <base-ref> and HEAD.
# Usage: eng/check-suppressions.sh origin/main   (CI: check out with fetch-depth: 0)
set -euo pipefail

base="${1:?usage: eng/check-suppressions.sh <base-ref>}"
diff_file="$(mktemp)"
trap 'rm -f "$diff_file"' EXIT

# No base ref → git fails and so does the step. A check that can't run must not pass.
git diff "$base"...HEAD -U0 -- . ':(exclude)eng/check-suppressions.sh' > "$diff_file"

added='#pragma warning disable|<NoWarn>|SuppressMessage|severity *= *(none|silent|suggestion)|Skip *='
removed='\[(Fact|Theory)'
status=0

if grep -E "^\+.*($added)" "$diff_file"; then
  echo "New suppression or skipped test: needs a reviewer."
  status=1
fi
if grep -E "^-.*($removed)" "$diff_file"; then
  echo "Removed test: needs a reviewer."
  status=1
fi
exit "$status"
