# ⚡ NEXUS Quick-Start Guide

> **Get from zero to orchestrated multi-agent pipeline in 5 minutes.**

---

## What is NEXUS?

**NEXUS** (Network of EXperts, Unified in Strategy) turns AI specialists into a coordinated pipeline. Instead of activating agents one at a time and hoping they work together, NEXUS defines exactly who does what, when, and how quality is verified at every step. This specific slice is tailored for Unity game development and features specialized Unity engineers, architects, managers, and testers.

## Choose Your Mode

| I want to... | Use | Agents | Time |
|-------------|-----|--------|------|
| Build a complete game or vertical slice | **NEXUS-Full** | All | 12-24 weeks |
| Build a gameplay feature | **NEXUS-Sprint** | Core | 2-6 weeks |
| Do a specific task (bug fix, refactor) | **NEXUS-Micro** | 3-5 | 1-5 days |

---

## 🚀 NEXUS-Full: Start a Complete Project

**Copy this prompt to activate the full pipeline:**

```text
Activate Unity Agents Orchestrator in NEXUS-Full mode.

Project: [YOUR PROJECT NAME]
Specification: [DESCRIBE YOUR PROJECT OR LINK TO SPEC]

Execute the complete NEXUS pipeline:
- Phase 0: Discovery (Unity Product Strategist, Unity Product Manager)
- Phase 1: Strategy (Unity Architect)
- Phase 2: Foundation (Unity Architect, Unity Senior Engineer, Engineering Rapid Prototyper)
- Phase 3: Build (Dev↔QA loops — Unity Senior Engineer, Unity Code Reviewer, Unity Functionality Reviewer, Unity Test Engineer)
- Phase 4: Harden (Unity Test Engineer, Unity Functionality Reviewer, Unity Code Reviewer)
- Phase 5: Launch (Unity Product Manager, Marketing/Growth support agents if needed)
- Phase 6: Operate (Unity Product Strategist, Unity Product Manager, analytics support)

Quality gates between every phase. Evidence required for all assessments.
Maximum 3 retries per task before escalation.
```

---

## 🏃 NEXUS-Sprint: Build a Feature

**Copy this prompt:**

```text
Activate Unity Agents Orchestrator in NEXUS-Sprint mode.

Feature: [DESCRIBE WHAT YOU'RE BUILDING]
Timeline: [TARGET WEEKS]

Sprint team:
- Product/Strategy: Unity Product Manager, Unity Product Strategist
- Architecture: Unity Architect
- Engineering: Unity Senior Engineer
- QA/Review: Unity Code Reviewer, Unity Functionality Reviewer, Unity Test Engineer

Begin at Phase 1 with architecture and sprint planning.
Run Dev↔QA loops for all implementation tasks.
Unity Test Engineer test plan approval required before code merge.
```

---

## 🎯 NEXUS-Micro: Do a Specific Task

**Pick your scenario and copy the prompt:**

### New Gameplay Feature
```text
Activate Unity Product Manager to define spec and acceptance criteria for [FEATURE].
Then Unity Product Strategist prioritizes scope.
Unity Architect creates architecture and refactor plan.
Unity Senior Engineer implements on feature branch.
Unity Test Engineer designs and executes test plan.
Unity Code Reviewer and Unity Functionality Reviewer provide merge gate approval.
```

### Risky Refactor
```text
Activate Unity Architect to plan the refactor for [SYSTEM].
Include Unity Functionality Reviewer from the start to document Game Rules OC.
Unity Test Engineer writes regression tests before touching code.
Unity Senior Engineer implements the refactor.
Unity Functionality Reviewer confirms behavior parity.
```

### Fix a Unity Bug
```text
Activate Unity Senior Engineer to investigate and fix [BUG DESCRIPTION].
After fix, activate Unity Code Reviewer to review code changes.
Then activate Unity Test Engineer to verify the fix works in-editor and creates regression tests.
Unity Functionality Reviewer ensures no original behaviors were lost.
```

### Prototype Systems
```text
Activate Engineering Rapid Prototyper to build a proof-of-concept backend/service for [IDEA].
Ensure it supports the Unity tech stack.
Once validated, Unity Architect plans the integration into the game client.
```

---

## 📁 Strategy Documents

| Document | Purpose | Location |
|----------|---------|----------|
| **Master Strategy** | Complete NEXUS doctrine | `strategy/nexus-strategy.md` |
| **Phase Playbooks** | Step-by-step guides for all phases | `strategy/playbooks/*.md` |
| **Activation Prompts** | Ready-to-use agent prompts | `strategy/coordination/agent-activation-prompts.md` |
| **Handoff Templates** | Standardized handoff formats | `strategy/coordination/handoff-templates.md` |

---

## 🔑 Key Concepts in 30 Seconds

1. **Quality Gates** — No phase advances without evidence-based approval (tests, code review).
2. **Dev↔QA Loop** — Every task is built then tested by Unity Test Engineer; PASS to proceed, FAIL to retry (max 3)
3. **Handoffs** — Structured context transfer between Unity agents (e.g. Architect to Senior Engineer).
4. **Behavior Parity** — Unity Functionality Reviewer checks refactors against original game rules.
5. **Unity Orchestrator** — Pipeline controller managing the entire feature flow.

---

## 🎭 The Unity Agents at a Glance

```text
PRODUCT & STRATEGY         │ ARCHITECTURE & ENG.         │ TESTING & REVIEW
Unity Product Strategist   │ Unity Architect             │ Unity Test Engineer
Unity Product Manager      │ Unity Senior Engineer       │ Unity Code Reviewer
Unity Agents Orchestrator  │ Eng. Rapid Prototyper       │ Unity Functionality Reviewer
```

---

<div align="center">

**Start with a mode. Follow the playbook. Trust the pipeline.**

`strategy/nexus-strategy.md` — The complete doctrine

</div>
