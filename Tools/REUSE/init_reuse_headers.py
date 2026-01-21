#!/usr/bin/env python3
# REUSE HEADER NONSENSE
import os
import subprocess
import sys
from concurrent.futures import ThreadPoolExecutor, as_completed
from threading import Lock
import time

REPO_ROOT = os.getcwd()
TARGET_EXTENSIONS = {".cs", ".yml", ".yaml"}
DEFAULT_LICENSE = "MIT"
BOT_KEYWORDS = ["[bot]", "-bot-", "github-actions"]  # Common bot name patterns

# Thread-safe counters
processed_counter = 0
skipped_counter = 0
error_counter = 0
print_lock = Lock()
dry_run_mode = False

def print_thread_safe(message):
    """Thread-safe printing - only in dry-run mode"""
    if dry_run_mode:
        with print_lock:
            print(message)

def is_bot_author(author_info: str) -> bool:
    """Check if an author name or email belongs to a bot."""
    lower_info = author_info.lower()
    return any(keyword.lower() in lower_info for keyword in BOT_KEYWORDS)

def is_target_folder(folder_path):
    """
    Check if this folder OR any parent folder matches our criteria:
    - Exact name 'Resources' (case-insensitive)
    - Starts with 'Content.' (case-insensitive)
    """
    # Walk up the directory tree
    current_path = folder_path
    while current_path and current_path != REPO_ROOT and len(current_path) > len(REPO_ROOT):
        folder_name = os.path.basename(current_path)
        folder_lower = folder_name.lower()

        # Check if this folder matches our criteria
        if folder_lower == "resources" or folder_lower.startswith("content."):
            return True

        # Move up one directory
        current_path = os.path.dirname(current_path)

    return False

def has_reuse_header(content):
    """Check if file already has a REUSE header"""
    # Check first 2KB for speed
    check_content = content[:2048] if len(content) > 2048 else content
    return "SPDX-License-Identifier:" in check_content

def get_git_authors(filepath):
    """Get git authors and emails for a file"""
    try:
        abs_path = os.path.abspath(filepath)
        # Get both name and email
        result = subprocess.run(
            ["git", "log", "--follow", "--format=%an|%ae|%ad", "--date=format:%Y", abs_path],
            stdout=subprocess.PIPE,
            stderr=subprocess.DEVNULL,
            text=True,
            cwd=REPO_ROOT,
            check=True,
        )
    except subprocess.CalledProcessError:
        return []
    except Exception as e:
        if dry_run_mode:
            print_thread_safe(f"Git error for {filepath}: {e}")
        return []

    authors = {}
    for line in result.stdout.splitlines():
        if "|" in line:
            parts = line.split("|", 2)
            if len(parts) == 3:
                name, email, year = parts
                name = name.strip()
                email = email.strip()
                year = year.strip()

                # Skip bot authors
                if is_bot_author(name) or is_bot_author(email):
                    if dry_run_mode:
                        print_thread_safe(f"  Skipping bot author: {name} <{email}>")
                    continue

                # Use author info as tuple (name, email)
                author_key = (name, email)
                authors.setdefault(author_key, set()).add(year)

    return [(author_info, sorted(years)) for author_info, years in authors.items()]

def format_years(years):
    """Format years into ranges like 2023-2025 or 2023, 2025"""
    if not years:
        return ""

    # Convert to integers and sort
    years_int = sorted(set(int(y) for y in years))
    ranges = []
    start = years_int[0]
    end = years_int[0]

    for year in years_int[1:]:
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

def build_header(ext, authors):
    comment = "//" if ext == ".cs" else "#"
    header = []

    for author_info, years in authors:
        name, email = author_info
        # Format years nicely
        year_str = format_years(years)

        # Format: // SPDX-FileCopyrightText: 2023-2025 Yellow <yellow@funkystation.org>
        header.append(f"{comment} SPDX-FileCopyrightText: {year_str} {name} <{email}>")

    header.append(f"{comment} SPDX-License-Identifier: {DEFAULT_LICENSE}")
    header.append("")

    return "\n".join(header)

def format_author_for_display(author_info):
    """Format author info for display in logs"""
    name, email = author_info
    return f"{name} <{email}>"

def process_single_file(args):
    filepath, dry_run = args
    global processed_counter, skipped_counter, error_counter

    try:
        ext = os.path.splitext(filepath)[1].lower()

        try:
            with open(filepath, "r", encoding="utf-8-sig") as f:
                content = f.read()
        except UnicodeDecodeError:
            try:
                with open(filepath, "r", encoding="utf-8") as f:
                    content = f.read()
            except UnicodeDecodeError:
                try:
                    with open(filepath, "r", encoding="latin-1") as f:
                        content = f.read()
                except Exception:
                    error_counter += 1
                    if dry_run_mode:
                        print_thread_safe(f"Encoding error: {filepath}")
                    return False

        # Check if already has header
        if has_reuse_header(content):
            skipped_counter += 1
            return True

        # Get authors
        authors = get_git_authors(filepath)
        if not authors:
            skipped_counter += 1
            if dry_run_mode:
                print_thread_safe(f"[SKIP] No non-bot authors found for: {filepath}")
            return True

        # Build header
        header = build_header(ext, authors)

        if dry_run:
            processed_counter += 1
            print_thread_safe(f"[DRY-RUN] Would update: {filepath}")
            author_list = [format_author_for_display(a[0]) for a in authors]
            print_thread_safe(f"  Authors: {', '.join(author_list)}")

            # Show year ranges for each author
            for author_info, years in authors:
                name, email = author_info
                year_str = format_years(years)
                print_thread_safe(f"    {name}: {year_str}")

            return True

        # Write file with header prepended - USE utf-8 (not utf-8-sig)
        try:
            with open(filepath, "w", encoding="utf-8") as f:
                f.write(header)
                f.write("\n")
                f.write(content)
            processed_counter += 1
            if dry_run_mode:
                author_list = [format_author_for_display(a[0]) for a in authors]
                print_thread_safe(f"  Authors added: {', '.join(author_list)}")
                # Show year ranges for each author
                for author_info, years in authors:
                    name, email = author_info
                    year_str = format_years(years)
                    print_thread_safe(f"    {name}: {year_str}")
            return True
        except Exception as e:
            error_counter += 1
            if dry_run_mode:
                print_thread_safe(f"Write error {filepath}: {e}")
            return False

    except Exception as e:
        error_counter += 1
        if dry_run_mode:
            print_thread_safe(f"Error processing {filepath}: {e}")
        return False

def collect_files_to_process():
    """Collect files from Content.*, Resources, and all their subfolders"""
    files_to_process = []

    if dry_run_mode:
        print(f"Scanning for files in Content.* and Resources folders (including subfolders)...")

    # Walk through the entire repository
    for root, dirs, files in os.walk(REPO_ROOT):
        # Skip .git directories
        if ".git" in root:
            continue

        # Check if this folder OR any parent is a target folder
        if is_target_folder(root):
            for file in files:
                ext = os.path.splitext(file)[1].lower()
                if ext in TARGET_EXTENSIONS:
                    full_path = os.path.join(root, file)
                    files_to_process.append(full_path)

    return files_to_process

def main():
    """Main function"""
    global dry_run_mode
    start_time = time.time()
    dry_run_mode = "--dry-run" in sys.argv

    # Parse thread count argument
    workers = 4  # Default reasonable number
    if "--workers" in sys.argv:
        idx = sys.argv.index("--workers")
        if idx + 1 < len(sys.argv):
            workers = int(sys.argv[idx + 1])

    if dry_run_mode:
        print(f"Running in DRY-RUN mode — no files will be modified.")
        print(f"Using {workers} worker threads")
        print(f"Repo root: {REPO_ROOT}")
        print(f"Ignoring bot authors containing: {', '.join(BOT_KEYWORDS)}")

    # Collect all files first
    files_to_process = collect_files_to_process()

    if not files_to_process:
        print("No files found in Content.* or Resources folders or their subfolders!")
        return

    if dry_run_mode:
        print(f"Found {len(files_to_process)} files to process")

    # Reset counters
    global processed_counter, skipped_counter, error_counter
    processed_counter = 0
    skipped_counter = 0
    error_counter = 0

    # Process files with ThreadPoolExecutor
    with ThreadPoolExecutor(max_workers=workers) as executor:
        # Submit all tasks
        futures = []
        for file in files_to_process:
            future = executor.submit(process_single_file, (file, dry_run_mode))
            futures.append(future)

        # Wait for all to complete
        completed = 0
        for future in as_completed(futures):
            completed += 1
            if dry_run_mode and completed % 100 == 0:
                print(f"Progress: {completed}/{len(files_to_process)} files processed")

    # Print summary (always print summary)
    end_time = time.time()
    elapsed_time = end_time - start_time

    print("\n" + "="*50)
    print("PROCESSING COMPLETE")
    print("="*50)
    print(f"Target folders: Content.* and Resources (including all subfolders)")
    print(f"Target extensions: {', '.join(sorted(TARGET_EXTENSIONS))}")
    print(f"Bot author patterns ignored: {', '.join(BOT_KEYWORDS)}")
    print(f"Total files found: {len(files_to_process)}")
    print(f"Files processed: {processed_counter}")
    print(f"Files skipped (already had headers/no non-bot authors): {skipped_counter}")
    print(f"Errors: {error_counter}")
    print(f"Time elapsed: {elapsed_time:.2f} seconds")

    if len(files_to_process) > 0:
        files_per_second = len(files_to_process) / elapsed_time
        print(f"Processing speed: {files_per_second:.2f} files/second")

    if dry_run_mode:
        print("\nNOTE: This was a dry run. No files were modified.")
        print("Run without --dry-run to apply changes.")
    else:
        print(f"\nSuccessfully updated {processed_counter} files with REUSE headers!")

if __name__ == "__main__":
    main()