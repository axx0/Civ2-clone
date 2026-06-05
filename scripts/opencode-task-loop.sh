#!/usr/bin/env bash
set -euo pipefail

REPO_DIR="/home/rhy/Projects/Civ2-clone"
MODEL="${MODEL:-ollama/qwen3:14b}"
AGENT="${AGENT:-build}"
OPENCODE_BIN="${OPENCODE_BIN:-/home/rhy/.opencode/bin/opencode}"

cd "$REPO_DIR"

while true; do
  task_file="$(find agent/tasks -maxdepth 1 -type f -name '*.md' | sort | head -n 1 || true)"

  if [ -z "$task_file" ]; then
    echo "No tasks found. Sleeping."
    sleep 60
    continue
  fi

  task_name="$(basename "$task_file" .md | tr -cd '[:alnum:]_.-')"
  stamp="$(date +%Y%m%d-%H%M%S)"
  log_file="agent/logs/${task_name}-${stamp}.log"

  echo "Starting task: $task_file"

  if [ -n "$(git status --porcelain -- . \
    ':(exclude)agent/tasks' \
    ':(exclude)agent/done' \
    ':(exclude)agent/failed' \
    ':(exclude)agent/logs')" ]; then
    echo "Working tree is dirty. Refusing to run." | tee "$log_file"
    sleep 300
    continue
  fi

  prompt="$(cat "$task_file")

Repository: /home/rhy/Projects/Civ2-clone

Instructions:
- Work only on the task described above.
- Make the smallest safe change.
- Do not edit secrets, credentials, .env files, or unrelated files.
- Do not push to remotes.
- Run ./scripts/quality_gate.sh before claiming success.
- If the quality gate fails, try to fix it.
- If blocked, stop and explain why.
- At the end, summarize files changed and checks run."

  set +e
  "$OPENCODE_BIN" run \
    --model "$MODEL" \
    --agent "$AGENT" \
    --title "agent:${task_name}" \
    "$prompt" 2>&1 | tee "$log_file"
  opencode_status="${PIPESTATUS[0]}"
  set -e

  if ./scripts/quality_gate.sh; then
    if git diff --quiet && git diff --cached --quiet; then
      echo "No changes produced." > "agent/failed/${task_name}.report.txt"
      mv "$task_file" "agent/failed/${task_name}.md"
      git add agent/failed agent/tasks || true
      git commit -m "agent: record no-change task ${task_name}" || true
      continue
    fi

    git add -A
    git commit -m "agent: ${task_name}"
    mv "$task_file" "agent/done/${task_name}.md"
    git add agent/done agent/tasks || true
    git commit -m "agent: mark ${task_name} done" || true
    echo "Task completed: $task_name"
  else
    echo "OpenCode status: $opencode_status. Quality gate failed. See $log_file" > "agent/failed/${task_name}.report.txt"
    mv "$task_file" "agent/failed/${task_name}.md" || true
    git add agent/failed agent/tasks || true
    git commit -m "agent: record failed task ${task_name}" || true
  fi
done
