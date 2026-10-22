#!/usr/bin/env bash
# The C# files that changed most in the last 12 months. Top of this list × hard to read = refactor there.
# Run from the repository root (on Windows: Git Bash). Optional argument: how many files to show.
set -euo pipefail
git log --since=12.months --format= --name-only -- '*.cs' \
    ':(exclude)Migrations/**' ':(exclude)**/Migrations/**' ':(exclude)**/*.Designer.cs' \
  | sort | uniq -c | sort -rn | head -"${1:-15}"
