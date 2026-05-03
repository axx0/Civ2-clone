---
description: "Gather context and structure planning for significant work. Run this command while in plan mode."
---

# Shape Spec

Gather context and structure planning for significant work. **Run this command while in plan mode.**

## Important Guidelines

- **Always use ask_user_v2 tool** when asking the user anything
- **Move one step at a time** — Don't overwhelm the user with all questions at once
- **Inject standards early** — Use `/inject-standards` to ensure the spec follows project rules

## Process

### Step 1: Understand the Goal

1. Read the user's initial prompt and any attached files.
2. Ask 2-3 focused questions to clarify:
   - What's the "Definition of Done"?
   - Are there specific technical constraints?
   - What are the biggest risks or unknowns?

### Step 2: Inject Relevant Standards

1. Run `/inject-standards` (auto-suggest mode).
2. Ask the user to confirm which standards should be applied to this spec.

### Step 3: Draft the Spec

1. Analyze the codebase to understand the current implementation.
2. Draft a structured spec including:
   - **Background** — Why are we doing this?
   - **Proposed Changes** — High-level technical approach
   - **Impact** — Which files/modules will be affected?
   - **Standards Applied** — Which standards are we following?
   - **Implementation Plan** — Step-by-step technical tasks
3. Confirm with the user and save to `agent-os/specs/[YYYY-MM-DD]-[feature-name].md`.

### Step 4: Final Review

1. Once the spec is approved, ask if the user wants to start implementing Step 1 of the plan.

## Output Location

Specs: `agent-os/specs/[YYYY-MM-DD]-[feature-name].md`
- Use ISO date format
- Use kebab-case for feature name
