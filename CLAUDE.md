# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this repo is

A personal C# reference/practice repo for LeetCode and interview prep — not a production app. The single project (`InterviewPrep/InterviewPrep.csproj`, target `net10.0`, `Nullable` + `ImplicitUsings` enabled) builds up a running catalog of C# language features and collection APIs relevant to LeetCode.

`Program.cs` holds only top-level statements (required — a project can have exactly one file with top-level statements) and is just an ordered list of `XyzDemo.Run()` calls. Each topic lives in its own file as a `public static class XyzDemo { public static void Run() { ... } }` (e.g. `Arrays.cs` → `ArraysDemo`, `LinkedLists.cs` → `LinkedListsDemo`). Types shared across demos (`ListNode`, `TreeNode`) live in the file for the topic they belong to (`LinkedLists.cs`, `Trees.cs`) and are usable from any other file with no `using` needed, since nothing in this repo uses namespaces.

## Conventions

- A new topic gets its own new file (`NewTopic.cs`) with a `NewTopicDemo.Run()` method, plus one line added to `Program.cs` calling it in a sensible position relative to related topics. Don't fold new topics into an existing file's `Run()` method.
- Within a `Run()` method, each sub-topic gets its own `//` comment before the code that demonstrates it. Keep growing a topic's file section by section rather than replacing earlier examples.
- Every `Console.WriteLine` should be labeled with what it's printing and, where relevant, which method produced it (e.g. `$"HashSet after Add(6): {...}"`, not a bare `string.Join(...)`) — the point is a readable console trace when scanning output, not just a working demo.
- Prefer showing the LeetCode-relevant method (e.g. `TryPop`, `TryPeek`, custom comparer `Sort`) over the minimal/obvious one, since the goal is coverage of what's useful in interviews, not idiomatic production code.
- New C# syntax (pattern matching, records, primary constructors, nullable operators) is intentionally exercised here to learn it — don't "simplify" it back to older syntax.
- Note Big-O complexity in a comment for non-obvious operations (e.g. `List<T>.Contains` is O(n) vs `HashSet<T>.Contains` O(1)) — that contrast is often the actual point of the example.

## Build/run

- `dotnet build` / `dotnet run --project InterviewPrep` from the repo root.
- `dotnet format` applies the `.editorconfig` style rules.
- Roslyn analyzers are enabled (`EnableNETAnalyzers`, `AnalysisLevel=latest`). `CA1051`/`CA1050` (public fields, no namespace — matches LeetCode-provided types like `ListNode`/`TreeNode`) and `CA1304`/`CA1305`/`CA1310`/`CA1311` (culture-aware string/number-formatting overloads — `ToUpper()`/`StartsWith(x)`/`ToString()` etc. are the plain overloads you'd actually write in an interview) are suppressed in the `.csproj`. Don't "fix" either pattern back to the analyzer-preferred form.
- `dotnet format --verify-no-changes` (what the pre-commit hook and CI run) fails on *any* analyzer warning it can't auto-fix, not just formatting diffs — so a new warning-causing pattern needs an explicit `NoWarn` entry (with a reason comment) if it's intentional, the same way the two suppressions above were added.

## Formatting enforcement

- A pre-commit hook (`.githooks/pre-commit`) runs `dotnet format --verify-no-changes` and blocks the commit if anything's unformatted. It's version-controlled, but `core.hooksPath` is a local git config setting — a fresh clone needs to run `git config core.hooksPath .githooks` once to activate it.
- `.github/workflows/ci.yml` runs the same format check plus a build on every PR into `main` and on pushes to `main`, so formatting is enforced even without the local hook.
