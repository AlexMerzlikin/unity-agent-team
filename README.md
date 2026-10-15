## 🎮 Unity Agents Team

> **A focused slice of The Agency, purpose-built for Unity game development.**  
> This repo is heavily inspired by [msitarzewski/agency-agents](https://github.com/msitarzewski/agency-agents).
> This repo packages the Unity feature pipeline plus supporting agents and strategy docs so you can run high‑quality Unity workflows in isolation.

---

## 🔧 What’s in this repo?

- **Unity orchestrator & specialists** (in `specialized/`):
  - `unity-agents-orchestrator.md` – coordinates the full Unity feature pipeline on feature branches
  - `unity-product-manager.md` – turns ideas into testable Unity feature specs with acceptance criteria
  - `unity-product-strategist.md` – prioritizes and sequences Unity work for retention/monetization goals
  - `unity-architect.md` – designs robust Unity architectures and refactor plans
  - `unity-senior-engineer.md` – implements architected designs with performance/memory focus
  - `unity-architectural-reviewer.md` – architecture-rule, dependency-direction, folder, naming, Controller/View and module-depth compliance gate for diffs
  - `unity-code-reviewer.md` – correctness, performance, and maintainability reviews before merge
  - `unity-performance-reviewer.md` – deep, mobile‑first performance review of diffs (allocations, hot paths, rendering, physics, memory)
  - `unity-profiler-analyst.md` – analyzes Unity Profiler / Memory Profiler / Frame Debugger captures and delivers a ranked, metrics‑backed optimization plan
  - `unity-functionality-reviewer.md` – protects gameplay behaviour parity during refactors
  - `unity-test-engineer.md` – plays every test case in the Editor through the `unity` CLI, sweeps ultra‑narrow to ultra‑wide resolutions, and proves the run with screenshots and console logs
  - `scripts/` – Editor and runtime helpers the Test Engineer runs with `run_script` (Game view resolution, EventSystem tap reachability)

- **Rapid build support**:
  - `engineering/engineering-rapid-prototyper.md` – for Unity game backends/prototypes that support your Unity stack

- **Shared playbooks & strategy** (in `strategy/`):
  - `coordination/agent-operating-contract.md` - state, context, tool request, human approval, error, and handoff contract for reliable agents
  - `QUICKSTART.md` – how to run the NEXUS pipeline
  - `EXECUTIVE-BRIEF.md` – C‑suite overview of the NEXUS model
  - `nexus-strategy.md` + `playbooks/` + `runbooks/` – full multi‑phase orchestration doctrine you can adapt to Unity projects

- **Cross‑cutting agents**:
  - `marketing/` – growth, social, and content agents for launching Unity features/products
  - `testing/` – Unity QA agents (Evidence Collector, Reality Checker pre‑merge gate, Performance Benchmarker, Test Results Analyzer), plus API, tool, and workflow testing agents for the services around your game
  - `support/` – game analytics (KPI investigation correlated with code and remote config), support, finance, infra, legal, and executive‑summary agents
  - `examples/` – NEXUS Spatial example showing how many agents coordinate on a single initiative

All auxiliary agents are here to support the Unity pipeline (strategy, launch, analytics, support), so the **entire repo is fully Unity oriented**.

---

## ⚡ How to use these agents

### In Claude / Cursor style agents

1. **Copy this repo** into your local agents directory (for example):

```bash
git clone https://github.com/your-org/unity-agent-team.git
```

2. In a new chat, **activate a Unity persona** by pasting one of the markdown files or by referencing it:
   - For end‑to‑end work: “Act as the `UnityAgentsOrchestrator` from `specialized/unity-agents-orchestrator.md` and run a full feature pipeline for: [brief].”
   - For a specific phase: “Act as `UnityProductManager` and write a Unity feature spec for: [brief].”

3. Use the NEXUS quickstart in `strategy/QUICKSTART.md` as a template to script multi‑agent workflows around Unity work (e.g. discovery → architecture → implementation → QA → launch).

### In Claude Code

Claude Code loads every agent file under your project's `.claude/agents/` folder (subfolders included) as a dispatchable agent type. Copy in the agents your pipeline uses, keep the folder structure (the Test Engineer expects its helpers at `.claude/agents/specialized/scripts/`), and archive the rest as described below. Agent files are read when a session starts, so restart the session after adding, archiving, or restoring one.

---

## 🗄️ Keep `.claude/agents/` lean: archive agents you rarely use

Every agent in `.claude/agents/` costs something in **every** session, even when it never runs: its name and description are added to the agent list the model reads on each turn, and every extra entry makes picking the right agent a little noisier. Markdown files without agent frontmatter (playbooks, examples, reference docs) are parsed there too and earn nothing. Keep the folder to the agents your skills, pipeline, or team actually dispatch.

**Archive instead of deleting.** Move rarely used agents and non‑agent docs to a sibling `.claude/agents-archive/` folder, keeping their subfolders. Claude Code does not scan it, so archived files cost nothing per session, stay in version control, and come back with a single move:

```bash
mkdir -p .claude/agents-archive
git mv .claude/agents/design .claude/agents-archive/design
git mv .claude/agents/strategy .claude/agents-archive/strategy
git mv .claude/agents/examples .claude/agents-archive/examples

# restore one agent when a task needs it, then restart the session
git mv .claude/agents-archive/design/unity-design-ui-designer.md .claude/agents/design/
```

**What to keep vs archive**

| Keep in `.claude/agents/` | Archive candidates |
|---|---|
| The pipeline you run: orchestrator, product manager/strategist, architect, senior engineer, test engineer | `design/` agents nobody dispatches |
| The merge gate: architectural, code, performance, functionality reviewers | `marketing/` agents outside launch periods |
| The QA agents you actually call (`testing/`) and the profiler analyst | `support/` and `testing/` agents for services you do not run |
| Agents a skill or slash command dispatches by name | `strategy/` doctrine and `examples/` (not agents at all) |

**Rules of thumb**

- An agent earns its place when something dispatches it: a skill, the orchestrator, or a routine your team runs. Review the folder when the pipeline changes, and archive what nothing calls any more.
- Keep reference material out of `.claude/agents/`. Put shared rule catalogues and checklists in the calling skill's `references/` folder and let agents open them by section. If the pipeline needs the NEXUS doctrine, keep a one‑page extract next to the skill that runs it and archive `strategy/`.
- Archiving is a move, not a delete. Write down in the project README which folders are archived and why, so teammates know where to look.

---

## 📐 Agent conventions

- **Provided Context.** Reviewer agents and the architect accept a `## Provided Context` block from their caller (diff, ticket summary, game‑rule excerpts, rule statuses). When it is present they never re‑fetch the diff, the ticket, or CLAUDE.md, and read repository files only by line range to confirm a finding. The orchestrator fetches the review input once and dispatches the reviewers in parallel.
- **Compact returns.** Reviewers return a fixed `VERDICT` / `FINDINGS` block with `[BLOCK]` / `[WARN]` / `[NIT]` severities and `path/File.cs:LINE` evidence, with no introduction or closing, so the orchestrator can merge them cheaply.
- **Unity Editor access.** Agents drive the Editor through the official `unity` CLI (Pipeline package `com.unity.pipeline`) first and fall back to Unity MCP only when the CLI is unavailable or lacks a command. A command that runs and reports failure (compile errors, red tests) is a result, not a broken tool.
- **Model tiers.** Planning, architecture, and orchestration run on `claude-opus-5-5`; implementation, review, and QA on `claude-sonnet-5-5`; light content agents on `claude-sonnet-5-5` with `effort: low`.

---

## 🧩 Suggested Unity workflows

- **New gameplay feature**
  1. `unity-product-manager` → spec + acceptance criteria
  2. `unity-product-strategist` → priority & scope
  3. `unity-architect` → architecture + refactor plan
  4. `unity-senior-engineer` → implementation on `feature/*` branch
  5. `unity-test-engineer` → test plan + execution
  6. `unity-architectural-reviewer` + `unity-code-reviewer` + `unity-functionality-reviewer` → merge gate, dispatched in parallel with one shared `## Provided Context` block
  7. `unity-performance-reviewer` → deep perf review for diffs touching hot paths, rendering, UI, physics, or asset pipeline
  8. `unity-profiler-analyst` → when the Performance Reviewer requests on‑device validation, or to diagnose an existing regression from a Profiler/Memory Profiler/Frame Debugger capture

- **Risky refactor**
  - Keep `unity-functionality-reviewer` in the loop from the start, and use `unity-test-engineer` to codify the “game rules OC” into tests before touching code.

- **Mobile performance pass**
  - Capture on a mid‑tier Android device, hand the capture to `unity-profiler-analyst` for a ranked hotspot report, then use `unity-performance-reviewer` to gate the resulting fix PRs.

- **End‑to‑end feature orchestration**
  - Let `UnityAgentsOrchestrator` drive the full pipeline and call out into marketing, testing, and support agents as you approach launch.

---

## 📂 Repo layout (high level)

```text
NewRepo/
  .github/                # PR + issue templates
  engineering/
    engineering-rapid-prototyper.md
  specialized/
    unity-*.md            # Unity orchestrator + specialists
    scripts/              # run_script helpers for the Unity Test Engineer
  strategy/               # NEXUS quickstart, playbooks, runbooks, coordination docs
  marketing/              # Growth + social agents (optional, but useful)
  testing/                # Testing agents for services around Unity
  support/                # Support/ops/analytics/legal/finance agents
  examples/               # Multi‑agent usage examples
  LICENSE
  CONTRIBUTING.md
  .gitignore
```

Use this repo as a **Unity‑focused slice** of the main `agency-agents` project: it keeps the Unity team self‑contained while still giving you the supporting agents and strategy material you’ll likely want around a real Unity project.
