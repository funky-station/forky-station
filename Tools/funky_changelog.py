#!/usr/bin/env python3

import os
import re
import sys
import yaml

CATEGORY = "Funklog"
PARTS_DIR = "Resources/Changelog/Parts/Funklog"

COMMENT_RE = re.compile(r"<!--.*?-->", re.DOTALL)
HEADER_RE = re.compile(r"^[ \t]*(?::cl:|\U0001F191)[ \t]*(.*)$", re.MULTILINE)
BULLET_RE = re.compile(r"^[ \t]*[-*][ \t]*(\S[^\r\n]*)$")
TYPE_RE = re.compile(r"^(add|remove|tweak|fix)[ \t]*:[ \t]*(\S[^\r\n]*)$", re.IGNORECASE)

TYPE_NAMES = {
    "add": "Add",
    "remove": "Remove",
    "tweak": "Tweak",
    "fix": "Fix",
}


def extract_changes(body: str):
    body = COMMENT_RE.sub("", body)

    header_match = HEADER_RE.search(body)
    if header_match is None:
        return None, []

    author_override = header_match.group(1).strip()

    rest = body[header_match.end():].lstrip("\r\n")
    changes = []
    for line in rest.splitlines():
        if not line.strip():
            continue
        if line.lstrip().startswith("#"):
            break

        bullet_match = BULLET_RE.match(line)
        text = bullet_match.group(1) if bullet_match else line.strip()
        if not text:
            continue

        type_match = TYPE_RE.match(text)
        if type_match:
            change_type = TYPE_NAMES[type_match.group(1).lower()]
            message = type_match.group(2).strip()
        else:
            change_type = "Tweak"
            message = text.strip()

        changes.append({"type": change_type, "message": message})

    return author_override, changes


def main():
    pr_body = os.environ.get("PR_BODY") or ""
    pr_author = os.environ["PR_AUTHOR"]
    pr_url = os.environ["PR_URL"]
    pr_number = os.environ["PR_NUMBER"]
    pr_merged_at = os.environ["PR_MERGED_AT"]

    author_override, changes = extract_changes(pr_body)

    github_output = os.environ.get("GITHUB_OUTPUT")

    if not changes:
        print("No :cl: changelog entries found in PR body, skipping.")
        if github_output:
            with open(github_output, "a", encoding="utf-8") as f:
                f.write("has_entry=false\n")
        return

    author = author_override if author_override else pr_author

    os.makedirs(PARTS_DIR, exist_ok=True)
    part_path = os.path.join(PARTS_DIR, f"pr-{pr_number}.yml")

    part_data = {
        "author": author,
        "category": CATEGORY,
        "changes": changes,
        "time": pr_merged_at,
        "url": pr_url,
    }

    with open(part_path, "w", encoding="utf-8") as f:
        yaml.safe_dump(part_data, f)

    print(f"Wrote {len(changes)} changelog entries to {part_path}")

    if github_output:
        with open(github_output, "a", encoding="utf-8") as f:
            f.write("has_entry=true\n")


if __name__ == "__main__":
    sys.exit(main())
