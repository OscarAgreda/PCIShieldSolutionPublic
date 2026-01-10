# ================================
# VS Code layout diagnostics script
# Safe: read-only, no changes made
# ================================
$ErrorActionPreference = "SilentlyContinue"

Write-Host "=== OS INFO ===" -ForegroundColor Cyan
Get-CimInstance -ClassName Win32_OperatingSystem |
    Select-Object Caption, Version, OSArchitecture

Write-Host "`n=== VS CODE EXECUTABLES FOUND ===" -ForegroundColor Cyan

$codeCandidates = @(
    "$env:LOCALAPPDATA\Programs\Microsoft VS Code\Code.exe",
    "$env:ProgramFiles\Microsoft VS Code\Code.exe",
    "$env:ProgramFiles(x86)\Microsoft VS Code\Code.exe",
    "$env:LOCALAPPDATA\Programs\Microsoft VS Code Insiders\Code - Insiders.exe",
    "$env:ProgramFiles\Microsoft VS Code Insiders\Code - Insiders.exe"
)

$codeInfo = $codeCandidates |
    Where-Object { Test-Path $_ } |
    ForEach-Object {
        $v = (Get-Item $_).VersionInfo
        [PSCustomObject]@{
            Path           = $_
            ProductVersion = $v.ProductVersion
            FileVersion    = $v.FileVersion
        }
    }

if ($codeInfo) {
    $codeInfo | Format-Table -AutoSize
} else {
    Write-Host "No VS Code executables found in the usual locations." -ForegroundColor Yellow
}

Write-Host "`n=== 'code' ON PATH (IF ANY) ===" -ForegroundColor Cyan

$codeCmd = Get-Command code -ErrorAction SilentlyContinue
if ($codeCmd) {
    $codeCmd | Select-Object Name, Source, Version | Format-Table -AutoSize

    Write-Host "`ncode --version output:" -ForegroundColor Gray
    try {
        & code --version
    } catch {
        Write-Host "Running 'code --version' failed: $($_.Exception.Message)" -ForegroundColor Yellow
    }
} else {
    Write-Host "No 'code' command found on PATH." -ForegroundColor Yellow
}

Write-Host "`n=== CANDIDATE settings.json FILES ===" -ForegroundColor Cyan

$settingsCandidates = @(
    (Join-Path $env:APPDATA "Code\User\settings.json"),
    (Join-Path $env:APPDATA "Code - Insiders\User\settings.json")
)

$settingsSummary = $settingsCandidates |
    ForEach-Object {
        [PSCustomObject]@{
            Path   = $_
            Exists = Test-Path $_
            SizeKB = if (Test-Path $_) {
                [Math]::Round((Get-Item $_).Length / 1KB, 2)
            } else {
                $null
            }
        }
    }

$settingsSummary | Format-Table -AutoSize

Write-Host "`n=== LAYOUT-RELATED SETTINGS (IF PRESENT) ===" -ForegroundColor Cyan

foreach ($path in $settingsCandidates) {
    if (Test-Path $path) {
        Write-Host "`n--- Snippet from $path ---" -ForegroundColor Green

        Get-Content $path |
            Select-String -Pattern `
                '"workbench.panel' , `
                '"workbench.sideBar' , `
                '"terminal.integrated.defaultLocation' , `
                '"terminal.integrated.defaultProfile' `
                -SimpleMatch
    }
}

Write-Host "`n=== DONE ===" -ForegroundColor Cyan
