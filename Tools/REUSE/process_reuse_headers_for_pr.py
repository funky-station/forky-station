#!/usr/bin/env python3
# REUSE HEADER NONSENSE - Always fixes incorrect headers
import os
import subprocess
import sys
from concurrent.futures import ThreadPoolExecutor, as_completed
from threading import Lock
import re

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

def extract_existing_headers(content):
    """Extract existing REUSE headers and rest of file"""
    lines = content.split('\n')
    headers = []
    rest_of_file = []
    in_header = False

    for line in lines:
        # Check if line is a REUSE header
        if line.strip().startswith(("// SPDX-", "# SPDX-")) or \
                (line.strip().startswith("//") and "SPDX" in line) or \
                (line.strip().startswith("#") and "SPDX" in line):
            headers.append(line)
            in_header = True
        elif in_header and (line.strip() == "" or line.strip().startswith(("//", "#"))):
            # Empty line or comment after headers
            headers.append(line)
        else:
            # End of header section
            rest_of_file.append(line)
            in_header = False

    return '\n'.join(headers), '\n'.join(rest_of_file)

def parse_existing_header(header_line):
    """Parse a header line to extract year and name"""
    pattern = r'^\s*(//|#)\s+SPDX-FileCopyrightText:\s+(\d{4}(?:-\d{4})?(?:,\s*\d{4}(?:-\d{4})?)*)\s+(.+?)(?:\s+<(.+?)>)?$'
    match = re.match(pattern, header_line)
    if match:
        comment_char, years_str, name, email = match.groups()
        return comment_char.strip(), years_str.strip(), name.strip(), email
    return None, None, None, None

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

    # Convert to list and sort by earliest contribution year (chronological order)
    authors_list = [(author_info, sorted(years)) for author_info, years in authors.items()]

    # Sort authors by their first contribution year (oldest first)
    authors_list.sort(key=lambda x: x[1][0] if x[1] else 9999)

    return authors_list

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

def headers_match_git_history(existing_headers, git_authors):
    """Check if existing headers match git history"""
    if not existing_headers:
        return False

    # Count license identifiers - if there are multiple, something is wrong
    license_count = existing_headers.count("SPDX-License-Identifier:")
    if license_count > 1:
        return False

    # Parse existing headers
    existing_authors = []
    for line in existing_headers.split('\n'):
        if "SPDX-FileCopyrightText:" in line:
            comment_char, years_str, name, email = parse_existing_header(line)
            if comment_char and name:
                existing_authors.append((name, email, years_str))

    # Check if number of authors matches
    if len(existing_authors) != len(git_authors):
        return False

    # Check each author matches
    for (git_name, git_email, git_years), (existing_name, existing_email, existing_years) in zip(git_authors, existing_authors):
        # Check name and email match
        if git_name != existing_name:
            return False
        if git_email != existing_email:
            return False

        # Check years match
        git_year_str = format_years(git_years)
        if git_year_str != existing_years:
            # Try to parse and compare years
            git_year_set = set(git_years)
            # Parse existing years (could be "2024" or "2024-2025" or "2024, 2025")
            existing_year_set = set()
            for part in existing_years.replace(',', ' ').split():
                if '-' in part:
                    start, end = map(int, part.split('-'))
                    existing_year_set.update(str(y) for y in range(start, end + 1))
                elif part.isdigit():
                    existing_year_set.add(part)

            if git_year_set != existing_year_set:
                return False

    return True

def has_multiple_license_identifiers(headers_text):
    """Check if there are multiple SPDX-License-Identifier lines"""
    if not headers_text:
        return False

    count = 0
    for line in headers_text.split('\n'):
        if "SPDX-License-Identifier:" in line:
            count += 1
            if count > 1:
                return True
    return False

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

    # Get git authors
    git_authors = get_git_authors(filepath)
    if not git_authors:
        skipped += 1
        log(f"[SKIP] No non-bot authors found for {filepath}")
        return True

    # Extract existing headers if any
    existing_headers, rest_of_file = extract_existing_headers(content)

    # Build correct header
    correct_header = build_header(ext, git_authors)

    # Check if we need to update the file
    needs_update = False
    update_reason = ""

    if existing_headers:
        if has_multiple_license_identifiers(existing_headers):
            needs_update = True
            update_reason = "multiple license identifiers"
        elif not headers_match_git_history(existing_headers, git_authors):
            needs_update = True
            update_reason = "headers don't match git history"
        else:
            needs_update = False
    else:
        needs_update = True
        update_reason = "missing headers"

    # Show what we're doing
    author_list = [format_author_display(a[0]) for a in git_authors]

    if not needs_update:
        skipped += 1
        return True

    if dry_run:
        processed += 1
        log(f"[NEEDS UPDATE] {filepath}")
        log(f"  Reason: {update_reason}")
        log(f"  Authors: {', '.join(author_list)}")
        return True

    # Update the file
    log(f"[UPDATING] {filepath}")
    log(f"  Reason: {update_reason}")
    log(f"  Authors: {', '.join(author_list)}")

    if existing_headers:
        # Replace existing headers
        new_content = correct_header + '\n' + rest_of_file
    else:
        # Add new headers
        new_content = correct_header + '\n' + content

    try:
        with open(filepath, "w", encoding="utf-8") as f:
            f.write(new_content)
        processed += 1
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
    print(f"Files updated/fixed: {processed}")
    print(f"Files skipped (already correct): {skipped}")
    print(f"Errors: {errors}")

    if dry_run:
        print(f"\nBot patterns ignored: {', '.join(BOT_KEYWORDS)}")
        print("NOTE: This was a dry run. No files were modified.")
        if processed > 0:
            print("Files would need REUSE header updates.")
            sys.exit(1)

    if errors > 0:
        sys.exit(1)

if __name__ == "__main__":
    main()