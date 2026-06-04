# Security Policy

## Supported Versions

| Version | Supported          |
| ------- | ------------------ |
| 1.x     | :white_check_mark: |

## Reporting a Vulnerability

If you discover a security vulnerability, please report it responsibly:

1. **Do NOT** open a public GitHub issue
2. Use [GitHub Private Vulnerability Reporting](../../security/advisories/new) for this repository
3. Or email: security@juniorgolfkenya.org

Include:
- Description of the vulnerability
- Steps to reproduce
- Potential impact
- Suggested fix (if any)

We will acknowledge receipt within 48 hours and provide a detailed response within 7 days.

## Security Practices

- Dependencies are monitored via Dependabot
- Code is scanned with CodeQL on every push and PR
- Secrets are scanned automatically on push
- All credentials must use environment variables, never hardcoded values
