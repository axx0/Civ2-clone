#!/usr/bin/env python3
from http.server import BaseHTTPRequestHandler, HTTPServer
from urllib.parse import parse_qs
from pathlib import Path
import html
import re
import subprocess
import time

REPO = Path("/home/rhy/Projects/Civ2-clone")
TASKS = REPO / "agent" / "tasks"
DONE = REPO / "agent" / "done"
FAILED = REPO / "agent" / "failed"
SERVICE = "civ2-opencode-agent.service"
HOST = "127.0.0.1"
PORT = 8765

for path in (TASKS, DONE, FAILED):
    path.mkdir(parents=True, exist_ok=True)


def run(cmd):
    try:
        return subprocess.run(
            cmd,
            cwd=REPO,
            text=True,
            capture_output=True,
            timeout=8,
        )
    except Exception as e:
        class Result:
            returncode = 1
            stdout = ""
            stderr = str(e)
        return Result()


def service_status():
    active = run(["systemctl", "--user", "is-active", SERVICE]).stdout.strip()
    enabled = run(["systemctl", "--user", "is-enabled", SERVICE]).stdout.strip()
    return active or "unknown", enabled or "unknown"


def recent_logs():
    result = run([
        "journalctl",
        "--user",
        "-u",
        SERVICE,
        "-n",
        "20",
        "--no-pager",
    ])
    return result.stdout or result.stderr


def git_summary():
    status = run(["git", "status", "--short"]).stdout.strip()
    log = run(["git", "log", "--oneline", "-6"]).stdout.strip()
    return status or "Working tree clean", log


def current_activity():
    logs = recent_logs()
    lines = [line for line in logs.splitlines() if line.strip()]
    last = lines[-1] if lines else "No logs yet."

    phase = "Idle / unknown"
    for line in reversed(lines):
        if "Starting task:" in line:
            phase = line.split("Starting task:", 1)[1].strip()
            break
        if "No tasks found. Sleeping." in line:
            phase = "Idle: no pending tasks"
            break
        if "opencode run" in line:
            phase = "OpenCode is running"
            break
        if "Determining projects to restore" in line:
            phase = "Running dotnet restore/build"
            break
        if "Test run for" in line or "Starting test execution" in line:
            phase = "Running tests"
            break
        if "Passed!" in line:
            phase = "Tests passed"
            break
        if "failed" in line.lower() or "error" in line.lower():
            phase = "Possible failure; check logs"
            break

    ollama = run(["ollama", "ps"]).stdout.strip()
    if not ollama:
        ollama = "No active Ollama model shown by ollama ps."

    return phase, last, ollama


def list_files(path):
    files = sorted(path.glob("*.md"))
    return files


def safe_task_filename(title):
    slug = title.lower()
    slug = re.sub(r"[^a-z0-9]+", "-", slug).strip("-")
    if not slug:
        slug = "task"
    stamp = time.strftime("%Y%m%d-%H%M%S")
    return f"{stamp}-{slug[:48]}.md"


def activity_fragment():
    active, enabled = service_status()
    phase, last_log_line, ollama_status = current_activity()
    logs = recent_logs()
    service_class = "good" if active == "active" else "bad"

    return f"""<!doctype html>
<html>
<head>
  <meta charset="utf-8">
  <meta http-equiv="refresh" content="2">
  <style>
    body {{
      margin: 0;
      background: #171d24;
      color: #e8eef5;
      font-family: system-ui, sans-serif;
    }}
    .pill {{
      display: inline-block;
      padding: 4px 10px;
      border-radius: 999px;
      font-weight: 700;
      font-size: 13px;
    }}
    .good {{
      background: #123d2a;
      color: #77f0ad;
    }}
    .bad {{
      background: #4b1f24;
      color: #ff9aa8;
    }}
    code, pre {{
      background: #0d1117;
      border: 1px solid #2c3744;
      border-radius: 8px;
    }}
    code {{
      padding: 2px 5px;
    }}
    pre {{
      padding: 10px;
      overflow: auto;
      max-height: 220px;
      white-space: pre-wrap;
    }}
  </style>
</head>
<body>
  <p>Status: <span class="pill {service_class}">{html.escape(active)}</span> Enabled: <code>{html.escape(enabled)}</code></p>
  <p><strong>Phase:</strong> <code>{html.escape(phase)}</code></p>
  <p><strong>Latest log line:</strong></p>
  <pre>{html.escape(last_log_line)}</pre>
  <h3>Ollama</h3>
  <pre>{html.escape(ollama_status)}</pre>
  <h3>Last 20 journal lines</h3>
  <pre>{html.escape(logs)}</pre>
</body>
</html>"""


def page(message=""):
    active, enabled = service_status()
    status, log = git_summary()
    pending = list_files(TASKS)
    done = list_files(DONE)
    failed = list_files(FAILED)

    def file_list(files):
        if not files:
            return "<p class='muted'>None</p>"
        items = []
        for f in files[-20:]:
            items.append(f"<li><code>{html.escape(f.name)}</code></li>")
        return "<ul>" + "\n".join(items) + "</ul>"

    service_class = "good" if active == "active" else "bad"

    return f"""<!doctype html>
<html>
<head>
  <meta charset="utf-8">
  <title>Civ2 Agent Dashboard</title>
  <style>
    body {{
      font-family: system-ui, sans-serif;
      margin: 0;
      background: #101418;
      color: #e8eef5;
    }}
    header {{
      padding: 18px 24px;
      background: #18202a;
      border-bottom: 1px solid #2c3744;
    }}
    h1 {{
      margin: 0;
      font-size: 24px;
    }}
    main {{
      padding: 24px;
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 18px;
    }}
    section {{
      background: #171d24;
      border: 1px solid #2c3744;
      border-radius: 14px;
      padding: 18px;
      box-shadow: 0 8px 24px rgba(0,0,0,.2);
    }}
    .wide {{
      grid-column: 1 / -1;
    }}
    .pill {{
      display: inline-block;
      padding: 4px 10px;
      border-radius: 999px;
      font-weight: 700;
      font-size: 13px;
    }}
    .good {{
      background: #123d2a;
      color: #77f0ad;
    }}
    .bad {{
      background: #4b1f24;
      color: #ff9aa8;
    }}
    .muted {{
      color: #97a6b5;
    }}
    code, pre {{
      background: #0d1117;
      border: 1px solid #2c3744;
      border-radius: 8px;
    }}
    code {{
      padding: 2px 5px;
    }}
    pre {{
      padding: 14px;
      overflow: auto;
      max-height: 260px;
      white-space: pre-wrap;
    }}
    textarea, input {{
      width: 100%;
      box-sizing: border-box;
      background: #0d1117;
      color: #e8eef5;
      border: 1px solid #2c3744;
      border-radius: 8px;
      padding: 10px;
      font: inherit;
    }}
    textarea {{
      min-height: 170px;
    }}
    button {{
      margin-top: 10px;
      background: #3b82f6;
      color: white;
      border: 0;
      border-radius: 8px;
      padding: 10px 14px;
      font-weight: 700;
      cursor: pointer;
    }}
    button.secondary {{
      background: #334155;
      margin-right: 8px;
    }}
    .message {{
      color: #77f0ad;
      font-weight: 700;
    }}
    a {{
      color: #93c5fd;
    }}
  </style>
</head>
<body>
<header>
  <h1>Civ2 OpenCode Agent Dashboard</h1>
  <p class="muted">Local only: <code>http://127.0.0.1:{PORT}</code> · Activity panel updates every 2 seconds</p>
</header>

<main>
  <section>
    <h2>Agent Service</h2>
    <p>Status: <span class="pill {service_class}">{html.escape(active)}</span></p>
    <p>Enabled: <code>{html.escape(enabled)}</code></p>
    <form method="post" action="/service">
      <button class="secondary" name="action" value="start">Start</button>
      <button class="secondary" name="action" value="stop">Stop</button>
      <button class="secondary" name="action" value="restart">Restart</button>
    </form>
    <p class="message">{html.escape(message)}</p>
  </section>

  <section>
    <h2>Current Activity</h2>
    <iframe src="/activity" style="width:100%; height:520px; border:0; border-radius:8px;"></iframe>
  </section>

  <section>
    <h2>Add Task</h2>
    <form method="post" action="/add-task">
      <label>Title</label>
      <input name="title" placeholder="Fix city population display bug">
      <br><br>
      <label>Task</label>
      <textarea name="body" placeholder="Describe exactly what the agent should do."></textarea>
      <button type="submit">Add Task</button>
    </form>
  </section>

  <section>
    <h2>Pending Tasks</h2>
    {file_list(pending)}
  </section>

  <section>
    <h2>Completed Tasks</h2>
    {file_list(done)}
  </section>

  <section>
    <h2>Failed / No-change Tasks</h2>
    {file_list(failed)}
  </section>

  <section>
    <h2>Git</h2>
    <h3>Status</h3>
    <pre>{html.escape(status)}</pre>
    <h3>Recent Commits</h3>
    <pre>{html.escape(log)}</pre>
  </section>

  <section class="wide">
    <h2>Recent Agent Logs</h2>
    <pre>{html.escape(recent_logs())}</pre>
  </section>
</main>
</body>
</html>"""


class Handler(BaseHTTPRequestHandler):
    def do_GET(self):
        if self.path == "/activity":
            self.respond(activity_fragment())
            return
        self.respond(page())

    def do_POST(self):
        length = int(self.headers.get("Content-Length", "0"))
        data = self.rfile.read(length).decode("utf-8")
        fields = parse_qs(data)

        if self.path == "/add-task":
            title = fields.get("title", [""])[0].strip()
            body = fields.get("body", [""])[0].strip()

            if not title:
                title = "Untitled task"

            if not body:
                self.respond(page("Task body is required."))
                return

            filename = safe_task_filename(title)
            task_path = TASKS / filename

            task_text = f"""# {title}

{body}

Agent requirements:
- Make the smallest safe change.
- Run ./scripts/quality_gate.sh.
- Do not push.
- Summarize files changed and checks run.
"""
            task_path.write_text(task_text)
            self.respond(page(f"Added task: {filename}"))
            return

        if self.path == "/service":
            action = fields.get("action", [""])[0]
            if action in {"start", "stop", "restart"}:
                result = run(["systemctl", "--user", action, SERVICE])
                if result.returncode == 0:
                    self.respond(page(f"Service {action} requested."))
                else:
                    self.respond(page(result.stderr or f"Failed to {action} service."))
                return

        self.respond(page("Unknown action."))

    def respond(self, content):
        body = content.encode("utf-8")
        self.send_response(200)
        self.send_header("Content-Type", "text/html; charset=utf-8")
        self.send_header("Content-Length", str(len(body)))
        self.end_headers()
        self.wfile.write(body)

    def log_message(self, format, *args):
        return


if __name__ == "__main__":
    print(f"Agent dashboard running at http://{HOST}:{PORT}")
    HTTPServer((HOST, PORT), Handler).serve_forever()
