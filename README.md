# InterviewPrep

A personal C# reference for LeetCode / interview prep. Not a production app — `InterviewPrep/Program.cs` is a single growing file that demonstrates the C# language features and collection APIs most useful for solving LeetCode problems (arrays, `List<T>`, `Dictionary<T,K>`, `HashSet<T>`, `Stack<T>`, nullable operators, etc.), one topic at a time.

Also see [`AI_PROFICIENCY.md`](AI_PROFICIENCY.md) — a running reference of what I've learned about working effectively with Claude Code while building this repo.

## Running it

```bash
dotnet run --project InterviewPrep
```

Each line prints a labeled result so the console output doubles as a readable trace of what each method does.

## Formatting

This repo enforces C# style via `.editorconfig`:

- **Locally**: a pre-commit hook (`.githooks/pre-commit`) runs `dotnet format --verify-no-changes` before allowing a commit. Activate it once per clone with:
  ```bash
  git config core.hooksPath .githooks
  ```
- **On GitHub**: `.github/workflows/ci.yml` runs the same check plus a build on every PR and push to `main`.

See `CLAUDE.md` for conventions this repo follows when adding new topics.
