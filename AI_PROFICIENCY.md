# AI Proficiency Notes

A running reference of things I've learned about working effectively with Claude Code, in the same growing-reference spirit as `InterviewPrep/Program.cs` — one topic section at a time, kept even after the underlying task is done.

## Token efficiency

- Claude doesn't re-read a file it just edited itself — the edit tool already confirms success, so asking it to "double check by re-reading" just burns tokens.
- When a file changes on disk *outside* of Claude's own edits, it gets a diff-style reminder (changed lines + context) instead of the full file — already the efficient path, nothing to do differently.
- Point Claude at file paths/line ranges instead of pasting file contents into chat.
- `/clear` between unrelated tasks — context accumulates across a session, so starting fresh for a new topic avoids dragging old file contents forward.
- Batch related asks into one message instead of many small round trips.
- Put repeated instructions in `CLAUDE.md` instead of re-explaining preferences every session.
- Use `/fast` or a lighter model for simple/mechanical edits; save full reasoning for actual design/debugging work.

## CLAUDE.md vs. skills vs. hooks

Three different mechanisms, easy to conflate:

- **`CLAUDE.md`** — persistent instructions loaded into *every* session automatically. Good for conventions, build commands, gotchas. Keep it short — every line should be something Claude would otherwise get wrong.
- **Skills** — packaged multi-step instructions invoked on-demand (auto-matched to a task, or triggered with `/skill-name`). Good for repeatable workflows.
- **Hooks** — deterministic shell commands the *harness* runs on lifecycle events (e.g. after every edit), not something Claude can choose to skip. Good for enforcement (formatting, linting).
- A literal "run this before every git commit" requirement is **not** a Claude Code hook — Claude Code hooks fire on tool events (edits, bash calls), and matchers can't filter by command content (e.g. "only on `git commit`"). That's what a real **git hook** (`.git/hooks/pre-commit` or a tracked `.githooks/` dir) is for.

## Local enforcement vs. CI enforcement

Two different gates, worth having both:

- **Pre-commit hook** (local) — catches issues before a commit is even made, fast feedback, but lives in `.git/hooks/` by default which is *not* version-controlled.
- To version-control a hook: put the script in a tracked folder (e.g. `.githooks/`) and run `git config core.hooksPath .githooks`. Caveat — `core.hooksPath` itself is a local git config setting, so a fresh clone still needs to run that command once to activate it.
- **CI workflow** (`.github/workflows/*.yml`, triggered on `pull_request`/`push`) — enforced on GitHub's servers regardless of what's set up locally, so it's the backstop even if someone skips or bypasses the local hook.

## .editorconfig / analyzers / dotnet format — who actually runs what

`.editorconfig` is just a settings file — it does nothing by itself. It only takes effect when something reads it:

- **IDE (Rider/VS Code)** — applies rules live as you type or on manual reformat.
- **`dotnet format`** — rewrites files to match it, but only when run explicitly (not part of `dotnet build`/`dotnet run` by default).
- **`dotnet build`** — only fails on style rules that are *also* Roslyn analyzer diagnostics set to `error` severity (e.g. via `EnforceCodeStyleInBuild` or `dotnet_diagnostic.IDE0055.severity = error`). Otherwise a style violation just shows as an IDE squiggle, not a build failure.

## Repo/workflow judgment calls

- **Public vs. private for a solo practice repo**: leaning public is fine — low stakes, no sensitive data, and a visible reference doc of drilled concepts is a reasonable thing to have public. Flip anytime with `gh repo edit --visibility private`.
- **Direct-to-main vs. PR, solo repo**: direct-to-main is fine day-to-day; there's no one to review. A PR is worth doing once specifically to smoke-test a new CI workflow (branch protection / checks UI only show up on a PR), then go back to direct commits.
- **Branch naming**: `main` is the current default (GitHub switched in 2020); `git init` locally still defaults to `master` unless configured otherwise — `gh repo create --push` aligns it to `main` regardless of the local default.
- **Private repos require no paid plan** — free GitHub accounts have had unlimited private repos since 2019; the free tier only limits things like private-repo collaborator count and Actions minutes.
