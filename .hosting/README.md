# RepoScope hosting reference

Reference-only preparation, 2026-09-10. The inert `manifest.json` describes a future hosting intake; it changes no runtime, data, name, domain, account or deployment trigger. Existing workflows may still run on a future merge.

Keep repository analysis local. A static landing page or reviewed report does not require a public .NET API or uploading a user's repositories. RS1 defines an explicit publication allowlist using synthetic or deliberately reviewed/redacted output. Exclude local paths, private repository identity, raw Git material and unreviewed generated reports.

RS2 keeps the website's execution claims accurate: the local CLI/API and standalone HTML reports are distinct surfaces. RS3 prepares display-name changes separately from package, CLI, report-format and configuration identifiers. No new name or domain is selected here.

Read the current README and any applicable repository instructions before implementation. Root AGENTS.md and CLAUDE.md returned 404 in this pass; no replacement harness is introduced. Syntax: `python -m json.tool .hosting/manifest.json`. Later export changes require actual local CLI/report tests using synthetic fixtures; JSON validation is not runtime or privacy acceptance.

Preserve the previous report/build and stable download links before any publication. Keep private account receipts, credentials and unregistered naming candidates outside this repository. No paid backend is needed for this preparation plan.
