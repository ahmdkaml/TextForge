param(
    [Parameter(Position = 0)]
    [string]$BranchName = "ui-rewrite"
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

Write-Host "Switching to '$BranchName'..."
Run-Git @("switch", $BranchName)

Write-Host "Fetching latest ui-rewrite from origin..."
Run-Git @("fetch", "origin", "ui-rewrite")

Write-Host "Resetting '$BranchName' to exactly match origin/ui-rewrite..."
Run-Git @("reset", "--hard", "origin/ui-rewrite")

Write-Host ""
Write-Host "Local '$BranchName' is now exactly equal to origin/ui-rewrite." -ForegroundColor Green
