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

def is_bot_author(author_name: str, author_email: str = "") -> bool:
    """Check if an author name or email belongs to a bot."""
    lower_name = author_name.lower()
    lower_email = author_email.lower() if author_email else ""

    # Check name for bot patterns
    name_is_bot = any(keyword.lower() in lower_name for keyword in BOT_KEYWORDS)

    # Check email for bot patterns
    email_is_bot = any(keyword.lower() in lower_email for keyword in BOT_KEYWORDS) if author_email else False

    return name_is_bot or email_is_bot

def has_reuse_header(content: str) -> bool:
    return "SPDX-License-Identifier:" in content[:MAX_HEADER_SCAN]

def get_git_authors(filepath: str):
    try:
        # Get name, email, and year
        result = subprocess.run(
            [
                "git",
                "log",
                "--follow",
                "--format=%an|%ae|%ad",  # name|email|date
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

        parts = line.split("|", 2)
        if len(parts) != 3:
            continue

        name, email, year = parts
        name = name.strip()
        email = email.strip()
        year = year.strip()

        # Skip bot authors
        if is_bot_author(name, email):
            continue

        # Use tuple (name, email) as key
        author_key = (name, email)
        authors.setdefault(author_key, set()).add(year)

    return [(author_info, sorted(years)) for author_info, years in authors.items()]

def format_author_display(author_info):
    """Format author info for display in logs"""
    name, email = author_info
    return f"{name} <{email}>"

def format_years(years):
    """Format years into ranges like 2023-2025 or 2023, 2025"""
    if not years:
        return ""

    years = sorted(set(int(y) for y in years))
    ranges = []
    start = years[0]
    end = years[0]

    for year in years[1:]:
        if year == end + 1:
            end = year
        else:
            if start == end:
                ranges.append(str(start))
            else:
                ranges.append(f"{start}-{end}")
            start = year
            end = year

    if start == end:
        ranges.append(str(start))
    else:
        ranges.append(f"{start}-{end}")

    return ", ".join(ranges)

def build_header(ext: str, authors):
    comment = "//" if ext == ".cs" else "#"
    lines = []

    for author_info, years in authors:
        name, email = author_info
        # Format years nicely
        year_str = format_years(years)
        # Format: // SPDX-FileCopyrightText: 2023-2025 Yellow <yellow@funkystation.org>
        lines.append(
            f"{comment} SPDX-FileCopyrightText: {year_str} {name} <{email}>"
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
        author_list = [format_author_display(a[0]) for a in authors]
        log(f"  Authors: {', '.join(author_list)}")
        return True

    try:
        with open(filepath, "w", encoding="utf-8") as f:
            f.write(header)
            f.write("\n")
            f.write(content)
        processed += 1
        log(f"[UPDATED] {filepath}")
        author_list = [format_author_display(a[0]) for a in authors]
        log(f"  Authors: {', '.join(author_list)}")
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

    if dry_run:
        print(f"\nBot patterns ignored: {', '.join(BOT_KEYWORDS)}")

    if dry_run and processed > 0:
        print("\nREUSE headers missing on modified files.")
        sys.exit(1)

    if errors > 0:
        sys.exit(1)

if __name__ == "__main__":
    main()