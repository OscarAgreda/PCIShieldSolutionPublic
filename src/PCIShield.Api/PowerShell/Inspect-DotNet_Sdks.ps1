# dotnet-inspect.ps1
# Inspect installed .NET SDKs, runtimes, and workloads for .NET 9 and .NET 10.
# - Lists SDKs / runtimes / workloads
# - Flags previews/RCs
# - Flags anything outside majors 9 and 10
# - Detects "extra" older builds per Name+Major (runtimes) and per Major (SDKs)
# - Performs a "Health check" for 9/10 + recommended workloads

$ErrorActionPreference = "Stop"
$script:WorkloadCliAvailable = $true

function Invoke-DotNetSafe {
    param(
        [Parameter(Mandatory)]
        [string] $Arguments
    )

    $dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
    if (-not $dotnet) {
        Write-Error "The 'dotnet' CLI is not available on PATH. Install .NET or fix PATH first."
        return $null
    }

    try {
        & $dotnet $Arguments 2>$null
    }
    catch {
        Write-Error "Failed to run 'dotnet $Arguments': $($_.Exception.Message)"
        return $null
    }
}

function Get-DotNetSdks {
    $output = Invoke-DotNetSafe "--list-sdks"
    if (-not $output) { return @() }

    $output | ForEach-Object {
        if (-not $_) { return }

        # Example line:
        # 9.0.307 [C:\Program Files\dotnet\sdk]
        $parts = $_ -split '\s+'
        if ($parts.Count -lt 2) { return }

        $versionRaw = $parts[0]
        $path = $parts[-1].Trim('[', ']')

        $isPreview = $versionRaw -match '-(preview|rc)[0-9\.]*'
        $coreVersion = $versionRaw.Split('-')[0]

        try {
            $v = [version]$coreVersion
        }
        catch {
            $v = [version]"0.0.0.0"
        }

        [pscustomobject]@{
            Kind       = 'SDK'
            VersionRaw = $versionRaw
            Version    = $v
            Major      = $v.Major
            Minor      = $v.Minor
            Path       = $path
            IsPreview  = $isPreview
        }
    }
}

function Get-DotNetRuntimes {
    $output = Invoke-DotNetSafe "--list-runtimes"
    if (-not $output) { return @() }

    $output | ForEach-Object {
        if (-not $_) { return }

        # Example line:
        # Microsoft.NETCore.App 9.0.1 [C:\Program Files\dotnet\shared\Microsoft.NETCore.App]
        $parts = $_ -split '\s+'
        if ($parts.Count -lt 3) { return }

        $name       = $parts[0]
        $versionRaw = $parts[1]
        $path       = $parts[-1].Trim('[', ']')

        $isPreview   = $versionRaw -match '-(preview|rc)[0-9\.]*'
        $coreVersion = $versionRaw.Split('-')[0]

        try {
            $v = [version]$coreVersion
        }
        catch {
            $v = [version]"0.0.0.0"
        }

        [pscustomobject]@{
            Kind       = 'Runtime'
            Name       = $name
            VersionRaw = $versionRaw
            Version    = $v
            Major      = $v.Major
            Minor      = $v.Minor
            Path       = $path
            IsPreview  = $isPreview
        }
    }
}

function Get-DotNetWorkloads {
    $output = Invoke-DotNetSafe "workload list"
    if (-not $output) {
        $script:WorkloadCliAvailable = $false
        return @()
    }

    # Detect "dotnet-workload list does not exist" or similar messages
    if ($output -match 'dotnet-workload list does not exist') {
        Write-Warning "The 'dotnet workload' CLI is not available. Workloads may be Visual Studio–managed only."
        $script:WorkloadCliAvailable = $false
        return @()
    }

    $script:WorkloadCliAvailable = $true

    # Skip header lines if they exist ("Installed workloads", separator, etc.)
    $payload = $output | Select-Object -Skip 0

    $payload |
        Where-Object { $_ -and ($_ -notmatch '^\s*Installed workloads\s*$') -and ($_ -notmatch '^-+$') } |
        ForEach-Object {
            $line = $_.Trim()
            if (-not $line) { return }

            # Typical format: "maui  10.0.100 [...]" or similar
            $parts = $line -split '\s+', 2
            $workloadId = $parts[0]
            $rest = if ($parts.Count -gt 1) { $parts[1].Trim() } else { '' }

            [pscustomobject]@{
                Workload       = $workloadId
                AdditionalInfo = $rest
            }
        }
}

$expectedMajors = @(9, 10)
$runtimeNamesForHealth = @(
    'Microsoft.NETCore.App',
    'Microsoft.AspNetCore.App',
    'Microsoft.WindowsDesktop.App'
)

Write-Host "=== dotnet --info ===" -ForegroundColor Cyan
Invoke-DotNetSafe "--info"

Write-Host ""
Write-Host "=== SDKs (dotnet --list-sdks) ===" -ForegroundColor Cyan
$sdks = Get-DotNetSdks
$sdks | Sort-Object Version, VersionRaw | Format-Table Kind, VersionRaw, Major, Minor, Path -AutoSize

Write-Host ""
Write-Host "=== Runtimes (dotnet --list-runtimes) ===" -ForegroundColor Cyan
$runtimes = Get-DotNetRuntimes
$runtimes | Sort-Object Name, Version, VersionRaw | Format-Table Kind, Name, VersionRaw, Major, Minor, Path -AutoSize

Write-Host ""
Write-Host "=== Workloads (dotnet workload list) ===" -ForegroundColor Cyan
$workloads = Get-DotNetWorkloads
if (-not $script:WorkloadCliAvailable) {
    Write-Host "dotnet workload CLI is not available or managed externally (for example, by Visual Studio)." -ForegroundColor Yellow
}
elseif ($workloads.Count -eq 0) {
    Write-Host "No workloads reported by 'dotnet workload list'." -ForegroundColor Yellow
}
else {
    $workloads | Sort-Object Workload | Format-Table Workload, AdditionalInfo -AutoSize
}

# --- Analysis: previews / RC and versions outside 9/10 ---

Write-Host ""
Write-Host "=== SDKs: Preview / RC ===" -ForegroundColor Yellow
$sdkPreview = $sdks | Where-Object { $_.IsPreview }
if ($sdkPreview) {
    $sdkPreview | Format-Table VersionRaw, Path -AutoSize
} else {
    Write-Host "No SDK previews or RCs found."
}

Write-Host ""
Write-Host "=== Runtimes: Preview / RC ===" -ForegroundColor Yellow
$runtimePreview = $runtimes | Where-Object { $_.IsPreview }
if ($runtimePreview) {
    $runtimePreview | Format-Table Name, VersionRaw, Path -AutoSize
} else {
    Write-Host "No runtime previews or RCs found."
}

Write-Host ""
Write-Host "=== SDKs: Outside .NET 9 and .NET 10 ===" -ForegroundColor Yellow
$sdkOutside = $sdks | Where-Object { $expectedMajors -notcontains $_.Major -and $_.Version -ne [version]'0.0.0.0' }
if ($sdkOutside) {
    $sdkOutside | Format-Table VersionRaw, Major, Path -AutoSize
} else {
    Write-Host "No SDKs outside .NET 9 or .NET 10 (ignoring parse failures)."
}

Write-Host ""
Write-Host "=== Runtimes: Outside .NET 9 and .NET 10 ===" -ForegroundColor Yellow
$runtimeOutside = $runtimes | Where-Object { $expectedMajors -notcontains $_.Major -and $_.Version -ne [version]'0.0.0.0' }
if ($runtimeOutside) {
    $runtimeOutside | Format-Table Name, VersionRaw, Major, Path -AutoSize
} else {
    Write-Host "No runtimes outside .NET 9 or .NET 10 (ignoring parse failures)."
}

# --- For 9 and 10: identify older builds you might want to remove ---

Write-Host ""
Write-Host "=== SDKs: Extra 9/10 versions (older than latest per major) ===" -ForegroundColor Yellow
$sdkStable = $sdks | Where-Object { -not $_.IsPreview -and $expectedMajors -contains $_.Major }
$sdkLatestPerMajor = $sdkStable |
    Group-Object Major |
    ForEach-Object {
        $_.Group | Sort-Object Version -Descending | Select-Object -First 1
    }

$sdkExtras = $sdkStable | Where-Object { $sdkLatestPerMajor -notcontains $_ }
if ($sdkExtras) {
    $sdkExtras | Sort-Object Major, Version | Format-Table VersionRaw, Major, Path -AutoSize
} else {
    Write-Host "No older extra SDKs for .NET 9/10 detected. You appear to have only the latest per major."
}

Write-Host ""
Write-Host "=== Runtimes: Extra 9/10 versions (older than latest per Name+Major) ===" -ForegroundColor Yellow
$runtimeStable = $runtimes | Where-Object { -not $_.IsPreview -and $expectedMajors -contains $_.Major }
$runtimeLatestPerNameMajor = $runtimeStable |
    Group-Object Name, Major |
    ForEach-Object {
        $_.Group | Sort-Object Version -Descending | Select-Object -First 1
    }

$runtimeExtras = $runtimeStable | Where-Object { $runtimeLatestPerNameMajor -notcontains $_ }
if ($runtimeExtras) {
    $runtimeExtras | Sort-Object Name, Major, Version | Format-Table Name, VersionRaw, Major, Path -AutoSize
} else {
    Write-Host "No older extra runtimes for .NET 9/10 detected per Name+Major."
}

Write-Host ""
Write-Host "=== SUMMARY ===" -ForegroundColor Cyan
# Write-Host "SDKs total:      $($sdks.Count)"
# Write-Host "Runtimes total:  $($runtimes.Count)"
# Write-Host "Workloads total: $($workloads.Count)"

# --- HEALTH CHECK: .NET 9/10 + Blazor WASM + MAUI / ASP.NET Core ---

Write-Host ""
Write-Host "=== HEALTH CHECK (.NET 9/10 core SDK + runtimes) ===" -ForegroundColor Cyan

foreach ($m in $expectedMajors) {
    $sdkForMajor = $sdkStable | Where-Object { $_.Major -eq $m } | Sort-Object Version -Descending | Select-Object -First 1
    if ($sdkForMajor) {
        Write-Host ".NET $m SDK: OK (latest installed: $($sdkForMajor.VersionRaw))"
    } else {
        Write-Host ".NET $m SDK: MISSING (no non-preview SDK found)" -ForegroundColor Red
    }

    foreach ($rName in $runtimeNamesForHealth) {
        $rt = $runtimeStable |
            Where-Object { $_.Major -eq $m -and $_.Name -eq $rName } |
            Sort-Object Version -Descending |
            Select-Object -First 1

        if ($rt) {
            Write-Host ".NET $m runtime ($rName): OK (latest installed: $($rt.VersionRaw))"
        } else {
            Write-Host ".NET $m runtime ($rName): MISSING (no non-preview runtime for this name)" -ForegroundColor Red
        }
    }
}

Write-Host ""
Write-Host "=== HEALTH CHECK (Recommended workloads for Blazor WASM / MAUI, CLI side) ===" -ForegroundColor Cyan

$recommendedWorkloads = @(
    'wasm-tools',       # Blazor WebAssembly AOT / WebAssembly tools
    'maui',             # .NET MAUI umbrella workload
    'maui-android',
    'maui-ios',
    'maui-maccatalyst',
    'maui-windows'
)

if (-not $script:WorkloadCliAvailable) {
    Write-Host "Cannot inspect CLI workloads because 'dotnet workload' CLI is not available." -ForegroundColor Yellow
    Write-Host "If you need CLI workloads, ensure the .NET SDK with workload support is installed or use Visual Studio workloads."
} elseif ($workloads.Count -eq 0) {
    Write-Host "No workloads reported by 'dotnet workload list'. If you need Blazor WASM AOT or MAUI via CLI, consider running:" -ForegroundColor Yellow
    Write-Host "  dotnet workload install wasm-tools"
    Write-Host "  dotnet workload install maui"
} else {
    $installedIds = $workloads.Workload
    $present = $recommendedWorkloads | Where-Object { $installedIds -contains $_ }
    $missing = $recommendedWorkloads | Where-Object { $installedIds -notcontains $_ }

    if ($present.Count -gt 0) {
        Write-Host "Installed recommended workloads:" -ForegroundColor Green
        $present | ForEach-Object { Write-Host "  $_" }
    } else {
        Write-Host "No recommended workloads detected (wasm-tools / maui*)." -ForegroundColor Yellow
    }

    if ($missing.Count -gt 0) {
        Write-Host "Missing recommended workloads (if you need them):" -ForegroundColor Yellow
        $missing | ForEach-Object { Write-Host "  dotnet workload install $_" }
    } else {
        Write-Host "All recommended workloads (wasm-tools, maui*) appear to be installed."
    }
}

Write-Host ""
Write-Host "Inspect the HEALTH CHECK section above to see if anything is missing for .NET 9/10, Blazor WASM, MAUI, and ASP.NET Core." -ForegroundColor Green



Write-Host ""
Write-Host "=== REMINDER RUN ===" -ForegroundColor Cyan
Write-Host "dotnet workload install wasm-tools" -ForegroundColor Cyan
Write-Host "dotnet workload install maui" -ForegroundColor Cyan



function Test-DotNetTemplate {
    param(
        [Parameter(Mandatory)]
        [string] $Template,
        [Parameter()]
        [string] $Framework,
        [Parameter(Mandatory)]
        [string] $Name
    )

    $dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
    if (-not $dotnet) {
        Write-Host "dotnet CLI not found on PATH; skipping template test '$Template'." -ForegroundColor Red
        return
    }

    $tempRoot = Join-Path $env:TEMP "dotnet-template-smoketests"
    if (-not (Test-Path $tempRoot)) {
        New-Item -ItemType Directory -Path $tempRoot | Out-Null
    }

    $projectDir = Join-Path $tempRoot $Name
    if (Test-Path $projectDir) {
        Remove-Item $projectDir -Recurse -Force -ErrorAction SilentlyContinue
    }
    New-Item -ItemType Directory -Path $projectDir | Out-Null

    Push-Location $projectDir
    try {
        $args = @("new", $Template, "-n", $Name)
        if (-not [string]::IsNullOrWhiteSpace($Framework)) {
            $args += @("-f", $Framework)
        }

        Write-Host ""
        Write-Host "Running template test: dotnet $($args -join ' ')" -ForegroundColor Cyan
        & $dotnet @args
        $exit = $LASTEXITCODE

        if ($exit -ne 0) {
            Write-Host "Template '$Template' ($Framework) FAILED with exit code $exit." -ForegroundColor Red
            return
        }

        Write-Host "Template '$Template' ($Framework) created successfully." -ForegroundColor Green

        # Find the actual project or solution created by the template
        $target = Get-ChildItem -Path $projectDir -Recurse -Include *.sln,*.csproj -ErrorAction SilentlyContinue |
                  Sort-Object FullName |
                  Select-Object -First 1

        if (-not $target) {
            Write-Host "  Could not find a .csproj/.sln to restore/build; skipping restore/build check." -ForegroundColor Yellow
            return
        }

        $targetPath = $target.FullName
        Write-Host "Restoring '$targetPath'..." -ForegroundColor Green
        & $dotnet restore $targetPath
        $exit = $LASTEXITCODE
        if ($exit -ne 0) {
            Write-Host "  restore FAILED for '$targetPath' (exit $exit)." -ForegroundColor Red
            return
        }

        Write-Host "  restore OK. Running 'dotnet build' for '$targetPath'..." -ForegroundColor Green
        & $dotnet build $targetPath -nologo
        $exit = $LASTEXITCODE
        if ($exit -ne 0) {
            Write-Host "  build FAILED for '$targetPath' (exit $exit)." -ForegroundColor Red
        }
        else {
            Write-Host "  build OK for '$targetPath'." -ForegroundColor Green
        }
    }
    finally {
        Pop-Location
    }
}

Write-Host ""
Write-Host "=== TEMPLATE SMOKE TESTS (.NET 9 / 10) ===" -ForegroundColor Cyan

$dotnetCmd = Get-Command dotnet -ErrorAction SilentlyContinue
if (-not $dotnetCmd) {
    Write-Host "dotnet CLI not found on PATH; skipping all template tests." -ForegroundColor Yellow
}
else {
    # Blazor WebAssembly on .NET 9
    Test-DotNetTemplate -Template "blazorwasm" -Framework "net10.0"  -Name "TestBlazorWasmNet9"

    # Blazor WebAssembly on .NET 10
    Test-DotNetTemplate -Template "blazorwasm" -Framework "net10.0" -Name "TestBlazorWasmNet10"

    # MAUI Blazor (framework is implied by template)
    Test-DotNetTemplate -Template "maui-blazor" -Framework ""       -Name "TestMauiBlazor"
}

Write-Host ""
Write-Host "Inspect the HEALTH CHECK and TEMPLATE SMOKE TESTS sections above to verify .NET 9/10 + Blazor WASM + MAUI + ASP.NET Core are fully working." -ForegroundColor Green
