param(
    [Parameter(Mandatory = $true, Position = 0)]
    [string]$BranchName,

    [Parameter(Mandatory = $true, Position = 1)]
    [int]$IssueNumber
)

$ErrorActionPreference = "Stop"

function Run-Git {
    param(
        [Parameter(Mandatory = $true)]
        [string[]]$Arguments
    )

    & git @Arguments

    if ($LASTEXITCODE -ne 0) {
        Write-Host ""
        Write-Host "ERROR: git command failed." -ForegroundColor Red
        Write-Host "git $($Arguments -join ' ')" -ForegroundColor Red
        exit $LASTEXITCODE
    }
}

function Run-Gh {
    param(
        [Parameter(Mandatory = $true)]
        [string[]]$Arguments
    )

    & gh @Arguments

    if ($LASTEXITCODE -ne 0) {
        Write-Host ""
        Write-Host "ERROR: gh command failed." -ForegroundColor Red
        Write-Host "gh $($Arguments -join ' ')" -ForegroundColor Red
        exit $LASTEXITCODE
    }
}

# --------------------------------------------------
# 1. Check for changes
# --------------------------------------------------

$status = git status --porcelain

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Could not get git status." -ForegroundColor Red
    exit $LASTEXITCODE
}

if (-not $status) {
    Write-Host "No changes to commit."
    exit 0
}

# --------------------------------------------------
# 2. Create feature branch
# --------------------------------------------------

Write-Host "Creating branch '$BranchName'..."
Run-Git @("switch", "-c", $BranchName)

# --------------------------------------------------
# 3. Stage everything
# --------------------------------------------------

Write-Host "Staging changes..."
Run-Git @("add", ".")

# --------------------------------------------------
# 4. Commit
# --------------------------------------------------

Write-Host "Committing..."
Run-Git @("commit", "-m", "implementing $BranchName")

# --------------------------------------------------
# 5. Push feature branch
# --------------------------------------------------

Write-Host "Pushing branch..."
Run-Git @("push", "-u", "origin", $BranchName)

# --------------------------------------------------
# 6. Create PR
# --------------------------------------------------

Write-Host "Creating pull request..."

Run-Gh @(
    "pr",
    "create",
    "--base", "main",
    "--head", $BranchName,
    "--title", "implementing $BranchName [$IssueNumber]",
    "--body", "closes #$IssueNumber"
)

# --------------------------------------------------
# 7. Merge PR
# --------------------------------------------------

Write-Host "Merging pull request..."

Run-Gh @(
    "pr",
    "merge",
    $BranchName,
    "--merge",
    "--delete-branch"
)

# --------------------------------------------------
# 8. Switch back to main
# --------------------------------------------------

Write-Host "Switching to main..."
Run-Git @("switch", "main")

# --------------------------------------------------
# 9. Get latest changes
# --------------------------------------------------

Write-Host "Pulling latest main..."
Run-Git @("pull", "origin", "main")

# --------------------------------------------------
# 10. Delete local feature branch
# --------------------------------------------------

Write-Host ""
Write-Host "Successfully implemented issue #$IssueNumber." -ForegroundColor Green
Write-Host "Branch: $BranchName"
Write-Host "PR merged into: main"
