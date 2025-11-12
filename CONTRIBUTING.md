# CONTRIBUTING

## 🧭 Workflow
- Work on a dedicated branch for every change:
  - feature/your-feature-name
  - fix/short-description
- Never commit directly to `main`.
- `main` is a protected branch: all changes must go through a Pull Request (PR).
- Each PR requires at least one approved code review from a teammate before merge.
- Keep your local `main` updated using:
  - `git checkout main`
  - `git pull --rebase origin main`
- Before opening a PR:
  - Rebase your feature branch onto the latest `main`:  
    `git fetch origin`
    `git rebase origin/main`
- Push your branch and open a PR. Assign a reviewer and add short description.
- Merge only after approval and all checks pass.

### ✅ Commit messages (examples)
- Use short, prefixed messages:
  - feature: add player dash ability
  - fix: prevent null ref in PlayerController
  - docs: update README controls section
  - chore: upgrade Unity package X
  - refactor: simplify input handling

Keep the body brief when needed. Use imperative mood in subject line.

## 💻 Git quick reference (basic & essential)
| Command | Description |
| --- | --- |
| `git clone <repo>` | Clone the repository |
| `git checkout -b feature/name` | Create and switch to a new branch |
| `git checkout main` | Switch to main branch |
| `git fetch origin` | Fetch remote changes |
| `git pull --rebase origin main` | Update local main by rebasing |
| `git rebase origin/main` | Rebase current branch onto remote main |
| `git add <files>` | Stage changes |
| `git commit -m "type: short description"` | Commit staged changes |
| `git push origin feature/name` | Push branch to remote |
| `git status` | Show working tree status |
| `git log --oneline` | Show compact commit history |
| `git merge --no-ff feature/name` | Merge branch (used by repo UI or maintainer) |

## 🔍 Code review checklist
- Code compiles in Unity and runs basic scenarios.
- No obvious performance issues (allocations inside hot loops).
- Unity scene/prefab changes intentionally included and documented in PR.

## 💻 C# style for Unity (team rules)
- Variables, constants, and methods: camelCase (e.g., playerSpeed, calculateDamage()).
- Classes: PascalCase (e.g., PlayerController, EnemySpawner).
- Each public class must be in a file with the same name as the class: PlayerController.cs contains class PlayerController.
- Use 4 spaces for indentation.
- Add comments when intent is not obvious. Keep comments short and focused.
- Keep Unity lifecycle methods explicit and ordered where helpful (e.g., Awake, Start, Update).
- Prefer small methods and clear names over long, complex functions.

## 🔧 Handling merge conflicts after a rebase
Common workflow when rebasing a feature branch onto updated `main`:

1. Update and rebase:
   - `git fetch origin`
   - `git checkout feature/your-branch`
   - `git rebase origin/main`

2. If there is a conflict, Git will stop and mark conflicts in files. Resolve conflicts:
   - Open conflicted files, edit to resolve markers (<<<<< ===== >>>>>).
   - `git add <resolved-files>`
   - `git rebase --continue`

3. If you need to stop rebase:
   - `git rebase --abort`

4. After successful rebase, push your branch. Because history changed, use:
   - `git push --force-with-lease origin feature/your-branch`

Notes:
- Review each conflict carefully; prefer the minimal, correct change.
- If unsure, ask the reviewer/author of the conflicting code in the PR thread before forcing a push.

## 🔁 Branch naming and PR tips
- Keep branches focused and small.
- Name branches clearly: feature/weapon-aim, fix/player-null-ref.
- Add a short description and testing steps to the PR.
- Mention related Jira/task ID if applicable (optional).

## ✅ Merge rules
- No direct pushes to `main`.
- Merge only via PR after passing checks and at least one approval.
- Use the repository UI to merge (squash or merge strategy per repo settings).
- If forced push to a branch is necessary, communicate to reviewers and use --force-with-lease.

## Summary
- Use feature/fix branches; never push directly to main.
- Keep local main updated with `git pull --rebase`.
- Open PRs, get at least one code review, and merge through the UI.
- Follow C# Unity style: camelCase for vars/methods/constants, PascalCase for classes, file name == class name, 4 spaces, comment unclear logic.
- Resolve rebase conflicts by editing files, git add, git rebase --continue, then git push --force-with-lease.

