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
  - `unity-code-reviewer.md` – correctness, performance, and maintainability reviews before merge
  - `unity-performance-reviewer.md` – deep, mobile‑first performance review of diffs (allocations, hot paths, rendering, physics, memory)
  - `unity-profiler-analyst.md` – analyzes Unity Profiler / Memory Profiler / Frame Debugger captures and delivers a ranked, metrics‑backed optimization plan
  - `unity-functionality-reviewer.md` – protects gameplay behaviour parity during refactors
  - `unity-test-engineer.md` – designs and runs Unity‑specific test plans against acceptance criteria

- **Rapid build support**:
  - `engineering/engineering-rapid-prototyper.md` – for Unity game backends/prototypes that support your Unity stack

- **Shared playbooks & strategy** (in `strategy/`):
  - `QUICKSTART.md` – how to run the NEXUS pipeline
  - `EXECUTIVE-BRIEF.md` – C‑suite overview of the NEXUS model
  - `nexus-strategy.md` + `playbooks/` + `runbooks/` – full multi‑phase orchestration doctrine you can adapt to Unity projects

- **Cross‑cutting agents**:
  - `marketing/` – growth, social, and content agents for launching Unity features/products
  - `testing/` – API, workflow, and performance testing agents that can wrap your Unity backends and services
  - `support/` – support, analytics, finance, infra, legal, and executive‑summary agents
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

---

## 🧩 Suggested Unity workflows

- **New gameplay feature**
  1. `unity-product-manager` → spec + acceptance criteria
  2. `unity-product-strategist` → priority & scope
  3. `unity-architect` → architecture + refactor plan
  4. `unity-senior-engineer` → implementation on `feature/*` branch
  5. `unity-test-engineer` → test plan + execution
  6. `unity-code-reviewer` + `unity-functionality-reviewer` → merge gate
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

