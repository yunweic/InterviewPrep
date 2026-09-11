# InterviewPrep

A personal C# reference for LeetCode / interview prep. Not a production app — each C# language feature or collection type useful for LeetCode (arrays, `List<T>`, `Dictionary<T,K>`, `HashSet<T>`, `Stack<T>`/`Queue<T>`, linked lists, trees, sorting, heaps, etc.) gets its own file with a `Run()` method, and `Program.cs` just calls each one in order.

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
