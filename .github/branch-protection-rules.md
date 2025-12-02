# Branch Protection Rules Configuration

## Main Branch Protection
Configure these settings for the `main` branch:

### Required Status Checks
- [x] Require status checks to pass before merging
- [x] Require branches to be up to date before merging

**Required checks:**
- `web-app`
- `mobile-app` 
- `api-service`
- `security-scan`
- `docker-build`
- `codeql`
- `sonarcloud`

### Pull Request Requirements
- [x] Require pull request reviews before merging
- **Required approving reviews:** 2
- [x] Dismiss stale reviews when new commits are pushed
- [x] Require review from code owners
- [x] Restrict pushes that create files larger than 100MB

### Additional Restrictions
- [x] Restrict pushes to matching branches
- [x] Allow force pushes: **Disabled**
- [x] Allow deletions: **Disabled**

## Dev Branch Protection
Configure these settings for the `dev` branch:

### Required Status Checks
- [x] Require status checks to pass before merging
- [x] Require branches to be up to date before merging

**Required checks:**
- `web-app`
- `mobile-app`
- `api-service`
- `security-scan`

### Pull Request Requirements
- [x] Require pull request reviews before merging
- **Required approving reviews:** 1
- [x] Dismiss stale reviews when new commits are pushed
- [x] Restrict pushes that create files larger than 100MB

### Additional Restrictions
- [x] Allow force pushes: **Disabled**
- [x] Allow deletions: **Disabled**

## Implementation Steps

1. Go to repository Settings → Branches
2. Click "Add rule" for each branch
3. Configure settings as specified above
4. Ensure all team members have appropriate permissions