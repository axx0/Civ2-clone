---
description: "Establish foundational product documentation through an interactive conversation. Creates mission, roadmap, and tech stack files."
---

# Plan Product

Establish foundational product documentation through an interactive conversation. Creates mission, roadmap, and tech stack files in `agent-os/product/`.

## Important Guidelines

- **Always use ask_user_v2 tool** when asking the user anything
- **Move one step at a time** — Don't overwhelm the user with all questions at once
- **Refine based on feedback** — Re-draft sections if the user isn't satisfied

## Process

### Step 1: Draft the Mission

1. Ask the user 1-2 questions about the product's core purpose, target audience, and primary problem solved.
2. Draft a concise Mission statement (1-3 paragraphs).
3. Confirm with the user and save to `agent-os/product/mission.md`.

### Step 2: Identify Tech Stack

1. Ask about the core technologies currently used or planned (frontend, backend, database, infra).
2. Ask about any non-negotiable tech choices or constraints.
3. Create `agent-os/product/tech-stack.md` with a clean list of technologies and their roles.

### Step 3: Outline Roadmap

1. Ask about 3-5 major milestones or features planned for the next 3-6 months.
2. Create `agent-os/product/roadmap.md` with a prioritized list of upcoming work.

### Step 4: Finalize

1. Ensure all files are scannable and concise.
2. Ask the user if they'd like to draft any other foundational documents (e.g., target-persona, competitive-landscape).

## Output Location

All files: `agent-os/product/`
- `mission.md`
- `tech-stack.md`
- `roadmap.md`
