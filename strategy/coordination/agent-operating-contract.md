# Agent Operating Contract

This repo follows the practical parts of the 12-factor agent approach: mostly normal software, explicit state, narrow LLM decisions, structured outputs, and human approval when the agent is about to cross a real boundary.

For Unity projects, the game remains deterministic. Quests, inventory, combat, economy, permissions, save data, and build/release state are owned by Unity code, project artifacts, tests, and humans. Agents may produce specs, plans, reviews, test scenarios, summaries, or proposed actions. They do not become the gameplay authority.

## Operating Model

Every agent step is a reducer:

```text
AgentRunState + bounded context -> AgentStepOutput
```

The orchestrator owns control flow. The LLM chooses only the next structured output for the current step. It does not run an unbounded loop, hide prompts in a framework, or silently mutate project state.

## AgentRunState

Use this as the resumable source of truth for a pipeline run.

```json
{
  "run_id": "unity-dash-2026-07-11",
  "mode": "NEXUS-Micro",
  "phase": "Implementation",
  "feature": "Player Dash",
  "branch": "feature/player-dash",
  "current_task": "Implement stamina-gated dash",
  "attempt": 1,
  "status": "in_progress",
  "artifacts": [
    "specs/player-dash.md",
    "architecture/player-dash-plan.md"
  ],
  "decisions": [
    "Dash consumes stamina before movement starts"
  ],
  "blockers": [],
  "next_action": "Request Unity Senior Engineer implementation"
}
```

Required fields: `run_id`, `mode`, `phase`, `feature`, `branch`, `current_task`, `attempt`, `status`, `artifacts`, `decisions`, `blockers`, `next_action`.

Allowed `status` values: `not_started`, `in_progress`, `waiting_for_human`, `blocked`, `failed`, `complete`.

## AgentStepOutput

Every agent response that advances work should fit this shape. Empty arrays are fine; missing intent is not.

```json
{
  "state_patch": {
    "phase": "QA",
    "current_task": "Validate stamina-gated dash",
    "attempt": 1,
    "next_action": "Run Unity Test Engineer scenarios"
  },
  "tool_requests": [],
  "human_requests": [],
  "error_summary": null,
  "handoff": null,
  "final_summary": null
}
```

Required fields: `state_patch`, `tool_requests`, `human_requests`, `error_summary`, `handoff`, `final_summary`.

## ToolRequest

Treat tools and specialist agents as structured outputs, not magic function calls.

```json
{
  "target_agent": "unity-test-engineer",
  "task": "Validate dash acceptance criteria in editor",
  "inputs": [
    "specs/player-dash.md",
    "implementation summary",
    "feature/player-dash"
  ],
  "expected_output": "PASS/FAIL report with evidence and reproduction steps",
  "acceptance_criteria": [
    "Dash triggers only when stamina is sufficient",
    "No console errors during the test scenario",
    "No obvious frame hitch in typical combat scene"
  ]
}
```

Required fields: `target_agent`, `task`, `inputs`, `expected_output`, `acceptance_criteria`.

## HumanRequest

Ask humans when the next step requires product judgment, scope change, release approval, credential access, paid services, destructive git actions, or acceptance of known risk.

```json
{
  "decision_needed": "Choose dash cancel behavior",
  "options": [
    "Dash cannot be canceled",
    "Dash can be canceled by dodge only"
  ],
  "recommended_option": "Dash cannot be canceled",
  "impact": "Simpler rules, easier animation and stamina testing",
  "default_if_unanswered": "Dash cannot be canceled"
}
```

Required fields: `decision_needed`, `options`, `recommended_option`, `impact`, `default_if_unanswered`.

## ErrorSummary

Compact failures before feeding them back into context. Do not paste raw logs unless the exact raw line matters.

```json
{
  "failed_step": "Unity Test Engineer PlayMode run",
  "symptom": "Dash test fails when stamina is exactly equal to cost",
  "evidence": "PlayMode test DashConsumesStamina failed on equality case",
  "likely_cause": "Implementation uses > instead of >=",
  "recovery_action": "Return to Unity Senior Engineer with equality-case fix request"
}
```

Required fields: `failed_step`, `symptom`, `evidence`, `likely_cause`, `recovery_action`.

## Context Rules

- Include only the current state, active artifacts, recent decisions, relevant errors, and the next requested output.
- Exclude stale phase history, unrelated agent reports, and raw logs once summarized.
- Keep prompts and reusable templates in this repo.
- Store progress as artifacts or `AgentRunState`, not as hidden chat memory.
- Resume by loading `AgentRunState` plus the listed artifacts, then emitting one `AgentStepOutput`.

## Unity Guardrails

- Product specs define intended player behavior.
- Unity architecture defines ownership and data flow.
- Tests and reviewers verify the implementation.
- Humans approve risky behavior changes, scope cuts, release decisions, paid actions, and destructive operations.
- LLMs may draft dialogue, interpret player intent, or suggest actions, but deterministic systems decide what actually happens in game.

