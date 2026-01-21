#!/usr/bin/env python3
# REUSE HEADER NONSENSE
import os
import subprocess
import sys
from concurrent.futures import ThreadPoolExecutor, as_completed
from threading import Lock

TARGET_EXTENSIONS = { ".cs", ".yml", ".yaml" }
DEFAULT_LICENSE = "MIT"
MAX_HEADER_SCAN = 4096
BOT_KEYWORDS = ["[bot]", "-bot-", "github-actions"]  # Common bot name patterns

processed = 0
skipped = 0
errors = 0
lock = Lock()

def log(msg):
    with lock:
        print(msg)

def is_bot_author(author_name: str) -> bool:
    """Check if an author name belongs to a bot."""
    lower_name = author_name.lower()
    return any(keyword.lower() in lower_name for keyword in BOT_KEYWORDS)

def has_reuse_header(content: str) -> bool:
    return "SPDX-License-Identifier:" in content[:MAX_HEADER_SCAN]

def get_git_authors(filepath: str):
    try:
        result = subprocess.run(
            [
                "git",
                "log",
                "--follow",
                "--format=%an|%ad",
                "--date=format:%Y",
                filepath,
            ],
            stdout=subprocess.PIPE,
            stderr=subprocess.DEVNULL,
            text=True,
            check=True,
        )
    except Exception:
        return []

    authors = {}
    for line in result.stdout.splitlines():
        if "|" not in line:
            continue
        name, year = line.split("|", 1)
        name = name.strip()

        # Skip bot authors
        if is_bot_author(name):
            continue

        authors.setdefault(name, set()).add(year.strip())

    return [(a, sorted(y)) for a, y in authors.items()]

def build_header(ext: str, authors):
    comment = "//" if ext == ".cs" else "#"
    lines = []

    for author, years in authors:
        for year in years:
            lines.append(
                f"{comment} SPDX-FileCopyrightText: {year} {author}"
            )

    lines.append(f"{comment} SPDX-License-Identifier: {DEFAULT_LICENSE}")
    lines.append("")
    return "\n".join(lines)

def read_file(filepath: str):
    for enc in ("utf-8-sig", "utf-8", "latin-1"):
        try:
            with open(filepath, "r", encoding=enc) as f:
                return f.read()
        except UnicodeDecodeError:
            continue
    raise UnicodeDecodeError("unknown", b"", 0, 1, "unable to decode")

def process_file(filepath: str, dry_run: bool):
    global processed, skipped, errors

    ext = os.path.splitext(filepath)[1].lower()
    if ext not in TARGET_EXTENSIONS:
        skipped += 1
        return True

    try:
        content = read_file(filepath)
    except Exception:
        errors += 1
        log(f"[ERROR] Encoding issue: {filepath}")
        return False

    if has_reuse_header(content):
        skipped += 1
        return True

    authors = get_git_authors(filepath)
    if not authors:
        skipped += 1
        log(f"[SKIP] No non-bot authors found for {filepath}")
        return True

    header = build_header(ext, authors)

    if dry_run:
        processed += 1
        log(f"[MISSING HEADER] {filepath}")
        log(f"  Authors: {[a[0] for a in authors]}")
        return True

    try:
        with open(filepath, "w", encoding="utf-8") as f:
            f.write(header)
            f.write("\n")
            f.write(content)
        processed += 1
        log(f"[UPDATED] {filepath}")
        log(f"  Authors: {[a[0] for a in authors]}")
        return True
    except Exception as e:
        errors += 1
        log(f"[ERROR] Write failed {filepath}: {e}")
        return False

def main():
    if len(sys.argv) < 2:
        print("Usage: reuse_pr_headers.py [--dry-run] <file> [file ...]")
        sys.exit(2)

    dry_run = "--dry-run" in sys.argv
    files = [f for f in sys.argv[1:] if not f.startswith("--")]

    if not files:
        print("No files provided.")
        sys.exit(0)

    with ThreadPoolExecutor(max_workers=4) as pool:
        futures = [
            pool.submit(process_file, f, dry_run)
            for f in files
        ]
        for _ in as_completed(futures):
            pass

    print("\n====== REUSE PR SUMMARY ======")
    print(f"Files checked: {len(files)}")
    print(f"Files needing headers: {processed}")
    print(f"Files skipped: {skipped}")
    print(f"Errors: {errors}")

    if dry_run and processed > 0:
        print("\nREUSE headers missing on modified files.")
        sys.exit(1)

    if errors > 0:
        sys.exit(1)

if __name__ == "__main__":
    main()