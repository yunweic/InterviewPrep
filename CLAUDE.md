# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this repo is

A personal C# reference/practice repo for LeetCode and interview prep — not a production app. The single project (`InterviewPrep/InterviewPrep.csproj`, target `net10.0`, `Nullable` + `ImplicitUsings` enabled) builds up a running catalog of C# language features and collection APIs relevant to LeetCode, all inside `InterviewPrep/Program.cs` using top-level statements.

## Conventions for Program.cs

- Each new topic (a collection type, a language feature) gets its own `//` comment header before the code that demonstrates it. Keep growing the file section by section rather than replacing earlier examples.
- Every `Console.WriteLine` should be labeled with what it's printing and, where relevant, which method produced it (e.g. `$"HashSet after Add(6): {...}"`, not a bare `string.Join(...)`) — the point is a readable console trace when scanning output, not just a working demo.
- Prefer showing the LeetCode-relevant method (e.g. `TryPop`, `TryPeek`, custom comparer `Sort`) over the minimal/obvious one, since the goal is coverage of what's useful in interviews, not idiomatic production code.
- New C# syntax (pattern matching, records, primary constructors, nullable operators) is intentionally exercised here to learn it — don't "simplify" it back to older syntax.

## Build/run

- `dotnet build` / `dotnet run --project InterviewPrep` from the repo root.
- `dotnet format` applies the `.editorconfig` style rules.
- Roslyn analyzers are enabled (`EnableNETAnalyzers`, `AnalysisLevel=latest`). `CA1051` (public fields) and `CA1050` (no namespace) are suppressed in the `.csproj` — LeetCode-provided types like `ListNode`/`TreeNode` intentionally use public fields with no namespace, so don't "fix" that pattern.
