---
description: "Rebuild and maintain the standards index file (index.yml)."
---

# Index Standards

Rebuild and maintain the standards index file (`index.yml`).

## Purpose

The index file provides a scannable map of all standards in the project. It helps AI agents quickly identify which standards are relevant to a task without reading every file.

## Process

### Step 1: Scan for Standards

1. Find all `.md` files in `agent-os/standards/` (including subfolders)
2. Exclude `README.md` if it exists

### Step 2: Compare with Current Index

1. Read `agent-os/standards/index.yml`
2. Identify:
   - **New files** — Files on disk not in the index
   - **Missing files** — Entries in the index for files no longer on disk
   - **Mismatched descriptions** — (Optional) Check if file content still matches description

### Step 3: Handle Discrepancies

For each discrepancy, use ask_user_v2 to resolve:

#### For New Files:
```
New standard found: database/migrations.md

Suggested description: "Rules for creating and running SQL migrations"

Accept this description? (yes / or type a better one)
```

#### For Missing Files:
```
Index entry exists for api/auth.md but the file is missing.

Remove from index? (yes / skip)
```

### Step 4: Write the Index

1. Generate the updated YAML structure
2. Organize by folder, then alphabetize by filename
3. Save to `agent-os/standards/index.yml`

## Format Example

```yaml
api:
  response-format:
    description: API response envelope structure and error format
  error-handling:
    description: Error codes and exception handling patterns
database:
  migrations:
    description: Rules for creating and running SQL migrations
global:
  naming:
    description: General naming conventions for files and variables
```
