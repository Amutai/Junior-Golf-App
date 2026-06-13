# Branch Protection Rules

## Main Branch (`main`)

| Setting | Value |
|---------|-------|
| Require PR before merging | ✅ |
| Required approving reviews | 1 |
| Dismiss stale reviews on new commits | ✅ |
| Require status checks to pass | ✅ |
| Required checks | `build-and-test`, `build-maui`, `Analyze` |
| Require branches up to date | ✅ |
| Enforce for admins | ✅ |
| Allow force pushes | ❌ |
| Allow deletions | ❌ |

## Develop Branch (`develop`)

| Setting | Value |
|---------|-------|
| Require PR before merging | ✅ |
| Required approving reviews | 1 |
| Dismiss stale reviews on new commits | ✅ |
| Require status checks to pass | ✅ |
| Required checks | `build-and-test` |
| Require branches up to date | ✅ |
| Enforce for admins | ❌ |
| Allow force pushes | ❌ |
| Allow deletions | ❌ |

## Notes

- Rules are enforced via GitHub API (configured programmatically)
- Status check names come from `.github/workflows/ci.yml` job names and `codeql.yml`
- Push protection blocks commits containing secrets regardless of branch rules
