



# List-Files-Names.ps1 — VS Code F5, self-contained config
# This script is fully autonomous: press F5 in VS Code and it runs using the config below.
# To change behavior, edit the values in $Config. No CLI params, no prompts.


# ==============================
# Developer Config (edit me)
# ==============================
# Path               : Root directory to scan. Default is your original path.
# Out                : Output format. One of: 'Table' | 'Json' | 'Csv'.
# Relative           : If $true, also emit PathRelative (relative path from $Config.Path).
# Hash               : If $true, compute SHA256 (slower on large trees).
# UseExtFilter       : If $true, only include files whose extensions are listed in OnlyExt.
# OnlyExt            : Array of extensions (with or without dot). E.g. @('php','css','svg').
# NameLike           : Wildcard filters on file name. Any match passes (OR semantics).
# NameNotLike        : Wildcard exclusion on file name. Any match excludes.
# MinSizeBytes       : Minimum file size (inclusive).
# MaxSizeBytes       : Maximum file size (inclusive).
# ModifiedAfter      : Include files modified at or after this timestamp (local time accepted).
# ModifiedBefore     : Include files modified at or before this timestamp (local time accepted).
# MaxDepth           : Optional integer depth limiter relative to $Config.Path (0 = top level only).
# IncludeHidden      : If $true, include hidden/system files.
# NoRecurse          : If $true, scan only the top directory (equivalent to MaxDepth = 0).
# SortBy             : Property used for sorting: Path|Ext|Type|SizeBytes|ModifiedUtc|Name.
# Desc               : If $true, sort descending.
# OutFile            : When set, persists output. For Table => CSV snapshot. For Json/Csv => that format.
# NamesOnly          : If $true, table shows only names (plus a few columns).
# NoSummary          : If $true, suppress the summary section in Table output.

# Oscar 
# if i want jsu the names of the files on the path 
# then make this     DumpMarkdown = $false  then either 
        # a) UseExtFilter    = $true    which will use the OnlyExt   
        # b) UseExtFilter    = $false    which will print all files on the path directory 

       # then DumpMarkdown = $true  this will cause   to print the content but make sure you leave the  UseExtFilter    = $false  

# si le pongo filtro entonces no puede imprimer el markdown, when there i a filter of true use php extension only then the markdown even thos us true then no print markdown

# oscar , si quiero que me impima el contenido de lo que tengo con changes, 
# UseExtFilter    = $false  
# OnlyExt         = @('php')   
# DumpMarkdown    = $true   
# DumpGitChangedOnly  = $true  

$Config = @{
    # --- Core scope ---
    Path            = 'C:\_tj\_a_php_crm\public'
    Out             = 'Table'           # 'Table' | 'Json' | 'Csv'

    # --- Meta columns ---
    Relative        = $false            # show PathRelative column
    Hash            = $false            # compute SHA256 hash

    # --- Extension filtering ---
    UseExtFilter    = $false            # when $true, restrict to OnlyExt
    OnlyExt         = @('php')          # accepts 'php' or '.php'

    # --- Name filters ---
    NameLike        = @()               # e.g. @('*config*','*.min.css')
    NameNotLike     = @()               # e.g. @('*.map','*.tmp')

    # --- Size & time windows ---
    MinSizeBytes    = 0
    MaxSizeBytes    = [long]::MaxValue
    ModifiedAfter   = (Get-Date '0001-01-01')
    ModifiedBefore  = (Get-Date '9999-12-31')

    # --- Depth and attributes ---
    MaxDepth        = $null             # e.g. 0 top-only, 1 includes one subdir level, etc.
    IncludeHidden   = $false            # include hidden/system files
    NoRecurse       = $false            # when $true, scan only the top directory

    # --- Sorting & persistence ---
    SortBy          = 'Path'            # Path|Ext|Type|SizeBytes|ModifiedUtc|Name
    Desc            = $false            # descending sort
    OutFile         = $null             # e.g. 'C:\temp\files.csv' or 'C:\temp\files.json'
    NamesOnly       = $false            # table view with only names (+ a few columns)
    NoSummary       = $false            # hide summary when $true

    # --- Markdown dump (opt-in) ---
    DumpMarkdown                    = $true         # when $true, also export file contents to Markdown
    DumpOutFile                     = "$env:TEMP\PROJECT_DUMP.md"  # where to write the dossier
    DumpIncludeBinaryLikeExtensions = $false         # include images/archives/office/pdf/etc
    DumpUseGitTrackedOnly           = $false         # when $true, restrict to `git ls-files` paths
    DumpGitChangedOnly              = $true         # when $true, Markdown dump only includes .php files with pending git changes
    DumpTitle                       = "Project Dossier" # H1 title
}
# ==============================
# End: Developer Config

# ==============================
# Engine (no user input needed)
# ==============================
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# ---------- Helpers ----------
function Normalize-Extensions([string[]]$exts) {
    $exts |
        Where-Object { -not [string]::IsNullOrWhiteSpace($_) } |
        ForEach-Object {
            $e = $_.Trim()
            if (-not $e.StartsWith('.')) { $e = '.' + $e }
            $e.ToLowerInvariant()
        }
}

function Get-ContentType([string]$ext) {
    switch ($ext) {
        '.html' { 'text/html' }
        '.htm'  { 'text/html' }
        '.css'  { 'text/css' }
        '.svg'  { 'image/svg+xml' }
        '.png'  { 'image/png' }
        '.jpg'  { 'image/jpeg' }
        '.jpeg' { 'image/jpeg' }
        '.woff' { 'font/woff' }
        '.woff2'{ 'font/woff2' }
        '.sqlite' { 'application/vnd.sqlite3' }
        default { 'application/octet-stream' }
    }
}


function Get-HtmlTitle([string]$p) {
    try {
        $raw = Get-Content -LiteralPath $p -Raw -ErrorAction Stop
        if ($raw.Length -gt 4096) { $raw = $raw.Substring(0, 4096) }
        $m = [regex]::Match($raw, '<title[^>]*>(.*?)</title\s*>', 'IgnoreCase,Singleline')
        if ($m.Success) { ($m.Groups[1].Value -replace '\s+', ' ').Trim() } else { $null }
    } catch { $null }
}
function Get-ImageSize([string]$p) {
    try {
        Add-Type -AssemblyName System.Drawing -ErrorAction SilentlyContinue | Out-Null
        $img = [System.Drawing.Image]::FromFile($p)
        try { "$($img.Width)x$($img.Height)" } finally { $img.Dispose() }
    } catch { $null }
}


function Get-SvgSize([string]$p) {
    try {
        $raw = Get-Content -LiteralPath $p -Raw -ErrorAction Stop
        $w = [regex]::Match($raw, 'width\s*=\s*"(?<w>[\d\.]+)', 'IgnoreCase').Groups['w'].Value
        $h = [regex]::Match($raw, 'height\s*=\s*"(?<h>[\d\.]+)', 'IgnoreCase').Groups['h'].Value
        if ($w -and $h) { return "$($w)x$($h)" }
        $vb = [regex]::Match($raw, 'viewBox\s*=\s*"(?<vb>[\d\.\s\-]+)"', 'IgnoreCase').Groups['vb'].Value
        if ($vb) {
            $parts = $vb -split '\s+'
            if ($parts.Count -ge 4) { return "$($parts[2])x$($parts[3])" }
        }
        $null
    } catch { $null }
}
function Get-CssLines([string]$p) {
    try { (Get-Content -LiteralPath $p -ErrorAction Stop | Measure-Object -Line).Lines } catch { $null }
}

function Get-SqlitePageSize([string]$p) {
    try {
        $fs = [System.IO.File]::Open($p, [System.IO.FileMode]::Open, [System.IO.FileAccess]::Read, [System.IO.FileShare]::ReadWrite)
        try {
            $buf = New-Object byte[] 100
            $null = $fs.Read($buf, 0, $buf.Length)
            $ps = ($buf[16] -shl 8) -bor $buf[17]
            if ($ps -eq 1) { $ps = 65536 }
            if ($ps -gt 0) { "pageSize: $ps" } else { $null }
        } finally { $fs.Dispose() }
    } catch { $null }
}
# --- New: helpers for Markdown dump ---
# --- New: helpers for Markdown dump ---
function Get-CodeFenceLanguage([string]$path) {
    $map = @{
        '.php'   = 'php';       '.phtml' = 'php';
        '.html'  = 'html';      '.htm'   = 'html';
        '.htaccess' = 'apache';
        '.css'   = 'css';
        '.js'    = 'javascript';'.ts'    = 'typescript';
        '.json'  = 'json';      '.xml'   = 'xml';
        '.yml'   = 'yaml';      '.yaml'  = 'yaml';
        '.sql'   = 'sql';
        '.md'    = 'markdown';  '.mdx'   = 'markdown';
        '.txt'   = 'text';
        '.ini'   = 'ini';       '.conf'  = 'ini'; '.env' = 'ini';
        '.csv'   = 'csv';
        '.twig'  = 'twig';
        '.vue'   = 'vue';
        '.sh'    = 'bash';      '.ps1'   = 'powershell'; '.bat' = 'batch';
        '.lock'  = 'text'
    }

    $ext = [System.IO.Path]::GetExtension($path)
    if ($map.ContainsKey($ext)) { return $map[$ext] }

    $name = [System.IO.Path]::GetFileName($path)
    if ($name -eq '.htaccess') { return 'apache' }

    'text'
}

function Get-FileSha1([string]$fullPath) {
    try {
        $sha = [System.Security.Cryptography.SHA1]::Create()
        $fs  = [System.IO.File]::OpenRead($fullPath)
        try {
            $hashBytes = $sha.ComputeHash($fs)
            ($hashBytes | ForEach-Object { $_.ToString('x2') }) -join ''
        } finally {
            $fs.Dispose()
            $sha.Dispose()
        }
    } catch {
        ''
    }
}

function Try-ReadText([string]$fullPath) {
    # Robust "is this actually text?" check, to avoid dumping binary into Markdown
    try {
        $bytes = [System.IO.File]::ReadAllBytes($fullPath)
    } catch {
        return $null
    }

    if ($bytes.Length -eq 0) {
        return ''
    }

    # Heuristic: sample first N bytes and look for NULs / control chars
    $sampleCount = [Math]::Min($bytes.Length, 4096)
    $nonTextCount = 0

    for ($i = 0; $i -lt $sampleCount; $i++) {
        $b = $bytes[$i]

        # NUL byte → very strong signal of binary
        if ($b -eq 0) {
            return $null
        }

        # Allow: tab(9), LF(10), CR(13)
        if ($b -lt 9 -or ($b -gt 13 -and $b -lt 32)) {
            $nonTextCount++
        }
    }

    # If >30% of sampled bytes are weird control chars, treat as binary-ish
    if ($nonTextCount -gt (0.30 * $sampleCount)) {
        return $null
    }

    # Safe-ish to decode as text
    try {
        return [System.Text.Encoding]::UTF8.GetString($bytes)
    } catch {
        try {
            return [System.Text.Encoding]::Default.GetString($bytes)
        } catch {
            return $null
        }
    }
}


# ---------- Load config into local variables (no prompts) ----------
# NOTE: We avoid casting on the left side of an assignment to prevent VS Code parsing errors.
$Path           = [string]          $Config.Path
$Out            = [string]          $Config.Out
$Relative       = [bool]            $Config.Relative
$Hash           = [bool]            $Config.Hash
$UseExtFilter   = [bool]            $Config.UseExtFilter
$OnlyExt        = [string[]]        $Config.OnlyExt
$NameLike       = [string[]]        $Config.NameLike
$NameNotLike    = [string[]]        $Config.NameNotLike
$MinSizeBytes   = [long]            $Config.MinSizeBytes
$MaxSizeBytes   = [long]            $Config.MaxSizeBytes
$ModifiedAfter  = [datetime]        $Config.ModifiedAfter
$ModifiedBefore = [datetime]        $Config.ModifiedBefore
$MaxDepth       =                   $Config.MaxDepth     # nullable int, may be $null
$IncludeHidden  = [bool]            $Config.IncludeHidden
$NoRecurse      = [bool]            $Config.NoRecurse
$SortBy         = [string]          $Config.SortBy
$Desc           = [bool]            $Config.Desc
$OutFile        = [string]          $Config.OutFile
$NamesOnly      = [bool]            $Config.NamesOnly
$NoSummary      = [bool]            $Config.NoSummary

# ---------- Input validation ----------
if (-not (Test-Path -LiteralPath $Path -PathType Container)) {
    throw "Path not found or not a directory: $Path"
}
switch ($Out) {
    'Table' { }
    'Json'  { }
    'Csv'   { }
    default { throw "Out must be 'Table','Json', or 'Csv'." }
}
if ($UseExtFilter -and $OnlyExt.Count -eq 0) {
    throw "When UseExtFilter = $true you must provide at least one extension in OnlyExt (e.g., @('php'))."
}
if ($ModifiedAfter -gt $ModifiedBefore) {
    throw "ModifiedAfter must be earlier than or equal to ModifiedBefore."
}
if ($MinSizeBytes -gt $MaxSizeBytes) {
    throw "MinSizeBytes must be <= MaxSizeBytes."
}

$validSort = @('Path','Ext','Type','SizeBytes','ModifiedUtc','Name')
if (-not ($validSort -contains $SortBy)) {
    throw "SortBy must be one of: $($validSort -join '|')."
}


# ---------- Gather files ----------
$gciParams = @{
    LiteralPath = $Path
    Recurse     = -not $NoRecurse
    File        = $true
    ErrorAction = 'SilentlyContinue'
    Force       = $IncludeHidden
}
$files = Get-ChildItem @gciParams

# ---------- Derived for depth calc ----------
$basePathNormalized = (Resolve-Path -LiteralPath $Path).Path.TrimEnd('\')

# Build extension set if requested
$extSet = $null
if ($UseExtFilter) {
    $normalizedExts = Normalize-Extensions -exts $OnlyExt

    # Avoid ambiguous ctor binding (single string -> capacity-int). Build empty set, then add.
    $extSet = [System.Collections.Generic.HashSet[string]]::new()
    foreach ($e in [string[]]$normalizedExts) {
        if ($e) { [void]$extSet.Add($e) }
    }
}

# Single-pass filtering (fast & 5.1-safe)
$files = $files | Where-Object {
    # --- Hard exclude: vendor tree ---
    if ($_.FullName -like 'C:\_tj\_a_php_crm\vendor\*') { return $false }

      if ($_.FullName -like 'C:\_tj\_a_php_crm\scripts\*') { return $false }

           if ($_.FullName -like 'C:\_tj\_a_php_crm\generated\*') { return $false }

    # Extension
    $ext = [string]$_.Extension
    $ext = $ext.ToLowerInvariant()
    if ($UseExtFilter -and -not $extSet.Contains($ext)) { return $false }

    # Size
    if ($_.Length -lt $MinSizeBytes -or $_.Length -gt $MaxSizeBytes) { return $false }

    # Modified window
    $m = $_.LastWriteTimeUtc
    if ($m -lt $ModifiedAfter.ToUniversalTime() -or $m -gt $ModifiedBefore.ToUniversalTime()) { return $false }

    # Depth (manual)
    if ($MaxDepth -ne $null) {
        $rel = $_.FullName.Substring($basePathNormalized.Length).TrimStart('\')
        if ($rel.Length -gt 0) {
            $depth = ($rel -split '\\').Where({ $_ -ne '' }).Count
        } else {
            $depth = 0
        }
        if ($depth -gt $MaxDepth) { return $false }
    }

    # Name includes / excludes
    $leaf = $_.Name
    if ($NameLike.Count -gt 0) {
        $any = $false
        foreach ($pat in $NameLike) { if ($leaf -like $pat) { $any = $true; break } }
        if (-not $any) { return $false }
    }
    if ($NameNotLike.Count -gt 0) {
        foreach ($pat in $NameNotLike) { if ($leaf -like $pat) { return $false } }
    }

    $true
}

# ---------- Projection ----------
$rows = $files | ForEach-Object {
    $full = $_.FullName

    $ext  = [string]$_.Extension
    $ext  = $ext.ToLowerInvariant()

    $type = switch ($ext) {
        '.html' { 'HTML' }
        '.htm'  { 'HTML' }
        '.png'  { 'Image' }
        '.jpg'  { 'Image' }
        '.jpeg' { 'Image' }
        '.svg'  { 'SVG' }
        '.css'  { 'CSS' }
        '.woff' { 'Font' }
        '.woff2'{ 'Font' }
        '.sqlite' { 'SQLite (file)' }
        default { if ($ext.Length -gt 0) { $ext.TrimStart('.') } else { 'Unknown' } }
    }

    $info = switch ($type) {
        'HTML'   { $t = Get-HtmlTitle -p $full; if ($t) { "title: $t" } }
        'Image'  { $d = Get-ImageSize -p $full; if ($d) { "dimensions: $d" } }
        'SVG'    { $d = Get-SvgSize   -p $full; if ($d) { "svg size: $d" } }
        'CSS'    { $n = Get-CssLines  -p $full; if ($n) { "lines: $n" } }
        'SQLite (file)' { Get-SqlitePageSize -p $full }
        default  { $null }
    }

    $hash = $null
    if ($Hash) {
        try { $hash = (Get-FileHash -LiteralPath $full -Algorithm SHA256).Hash } catch { $hash = $null }
    }

    $pathRel = $null
    if ($Relative) {
        try { $pathRel = Resolve-Path -LiteralPath $full -Relative } catch { $pathRel = $null }
    }

    if ($NamesOnly) {
        [pscustomobject]@{
            Name          = $_.Name
            Path          = $full
            PathRelative  = $pathRel
            Ext           = $ext
            Type          = $type
            SizeBytes     = $_.Length
            ModifiedUtc   = $_.LastWriteTimeUtc
            Info          = $info
            ContentType   = Get-ContentType $ext
            Sha256        = $hash
        }
    } else {
        [pscustomobject]@{
            Path          = $full
            PathRelative  = $pathRel
            Ext           = $ext
            Type          = $type
            ContentType   = Get-ContentType $ext
            SizeBytes     = $_.Length
            ModifiedUtc   = $_.LastWriteTimeUtc
            Info          = $info
            Sha256        = $hash
        }
    }
}

# ---------- Sorting ----------
$rows = if ($Desc) {
    $rows | Sort-Object -Property $SortBy -Descending
} else {
    $rows | Sort-Object -Property $SortBy
}

# ---------- Output ----------
switch ($Out) {
    'Table' {
        if ($NamesOnly) {
            $rows | Select-Object @{n='Name';e={$_.Name}}, @{n='Modified';e={ $_.ModifiedUtc.ToLocalTime() }}, SizeBytes, Ext, Type | Format-Table -AutoSize
        } else {
            $rows | Select-Object `
                @{n='Path';e={$_.Path}}, `
                @{n='Ext';e={$_.Ext}}, `
                @{n='Type';e={$_.Type}}, `
                @{n='SizeBytes';e={$_.SizeBytes}}, `
                @{n='Modified';e={ $_.ModifiedUtc.ToLocalTime() }}, `
                @{n='Info';e={$_.Info}} | Format-Table -AutoSize
        }

        if (-not $NoSummary) {
            "`nSummary by type:"
            $rows | Group-Object Type | Sort-Object Name | ForEach-Object {
                $sum = ($_.Group | Measure-Object -Property SizeBytes -Sum).Sum
                "{0,-15} {1,6} files, {2,12:N0} bytes" -f $_.Name, $_.Count, $sum
            }
            "`nSummary by extension:"
            $rows | Group-Object Ext | Sort-Object Name | ForEach-Object {
                $sum = ($_.Group | Measure-Object -Property SizeBytes -Sum).Sum
                "{0,-10} {1,6} files, {2,12:N0} bytes" -f $_.Name, $_.Count, $sum
            }

            # Robust totals even when $rows is 0 or 1 item (avoid .Count on a single PSObject)
            $rowsArray  = @($rows)
            $totalFiles = $rowsArray.Count

            # Robust numeric sum: project strictly to [long] before measuring
            $sizeVals = @(
                foreach ($r in $rowsArray) {
                    if ($null -ne $r -and $null -ne $r.SizeBytes) { [long]$r.SizeBytes }
                }
            )

            if ($sizeVals.Count -gt 0) {
                $totalBytes = [long](( $sizeVals | Measure-Object -Sum ).Sum)
            } else {
                $totalBytes = 0L
            }

            "`nTotal files: $totalFiles  Total bytes: $($totalBytes.ToString('N0'))"
        }

        if ($OutFile) {
            $rows | Export-Csv -NoTypeInformation -Path $OutFile -Encoding UTF8
            "`nSaved CSV snapshot to: $OutFile"
        }
    }
    'Json' {
        $json = $rows | ConvertTo-Json -Depth 6
        if ($OutFile) {
            $json | Out-File -LiteralPath $OutFile -Encoding UTF8
            "Saved JSON to: $OutFile"
        } else {
            $json
        }
    }
    'Csv'  {
        if ($OutFile) {
            $rows | Export-Csv -NoTypeInformation -Path $OutFile -Encoding UTF8
            "Saved CSV to: $OutFile"
        } else {
            $rows | ConvertTo-Csv -NoTypeInformation
        }
    }
}
# ---------- Optional Markdown dossier (uses the *same filtered set*) ----------
if ($Config.DumpMarkdown) {
    $dumpFile  = [string]$Config.DumpOutFile
    if ([string]::IsNullOrWhiteSpace($dumpFile)) {
        throw "DumpMarkdown is enabled but DumpOutFile is empty."
    }

    $dumpGitChangedOnly = [bool]$Config.DumpGitChangedOnly

    # Extensions to treat as "binary-like" / heavy assets (ported from Export-ProjectMarkdown.ps1)
    # These are *skipped* when DumpIncludeBinaryLikeExtensions = $false
    $binaryLikePattern = '(?i)\.(png|jpe?g|gif|bmp|ico|md|webp|svgz?|tiff?|mp3|wav|ogg|mp4|m4v|mov|avi|wmv|zip|7z|rar|gz|bz2|xz|pdf|docx?|xlsx?|pptx?|exe|dll|pdb|so|dylib|bin|iso|lock)$'

    # Build initial path set from the same filtered rows used for the table/JSON/CSV
    $allPaths = @($rows | ForEach-Object { $_.Path })

    # Optional: restrict to git-tracked files only
    if ($Config.DumpUseGitTrackedOnly) {
        $git = Get-Command git -ErrorAction SilentlyContinue
        if (-not $git) { throw "DumpUseGitTrackedOnly = true but 'git' was not found on PATH." }

        Push-Location -LiteralPath $Path
        try {
            $tracked = & git ls-files
            $trackedFull = @()
            foreach ($rel in $tracked) {
                $trackedFull += (Join-Path -Path $Path -ChildPath $rel)
            }

            $trackedSet = [System.Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
            foreach ($t in $trackedFull) { [void]$trackedSet.Add($t) }

            $allPaths = $allPaths | Where-Object { $trackedSet.Contains($_) }
        } finally {
            Pop-Location
        }
    }

    # Optional: restrict to .php files that have pending git changes
    if ($dumpGitChangedOnly) {
        $git = Get-Command git -ErrorAction SilentlyContinue
        if (-not $git) {
            throw "DumpGitChangedOnly = true but 'git' was not found on PATH."
        }

        # Find repo root from current configured Path
        Push-Location -LiteralPath $Path
        try {
            $repoRoot = (& git rev-parse --show-toplevel 2>$null).Trim()
        } finally {
            Pop-Location
        }

        if (-not $repoRoot) {
            throw "DumpGitChangedOnly = true but '$Path' is not inside a git repository."
        }

        # Get porcelain status from repo root (staged, unstaged, untracked)
        Push-Location -LiteralPath $repoRoot
        try {
            $statusLines = & git status --porcelain
        } finally {
            Pop-Location
        }

        $changedPhpRel = @()
        foreach ($line in $statusLines) {
            if ([string]::IsNullOrWhiteSpace($line)) { continue }
            if ($line.Length -lt 4) { continue }

            # Skip the 2-char status and space, keep the path portion
            $rest = $line.Substring(3).Trim()
            if (-not $rest) { continue }

            # Handle renames: "R100 old.php -> new.php"
            if ($rest -match ' -> ') {
                $parts = $rest -split ' -> '
                $rel   = $parts[-1]
            } else {
                $rel = $rest
            }

            if ($rel.ToLowerInvariant().EndsWith('.php')) {
                $changedPhpRel += $rel
            }
        }

        if ($changedPhpRel.Count -eq 0) {
            # No changed .php files => nothing to dump
            $allPaths = @()
        } else {
            # Map to full paths and build a case-insensitive set
            $changedPhpFull = @()
            foreach ($rel in ($changedPhpRel | Sort-Object -Unique)) {
                $changedPhpFull += (Join-Path -Path $repoRoot -ChildPath $rel)
            }

            $changedSet = [System.Collections.Generic.HashSet[string]]::new()
            foreach ($f in $changedPhpFull) {
                if (Test-Path -LiteralPath $f -PathType Leaf) {
                    try {
                        $res = (Resolve-Path -LiteralPath $f -ErrorAction Stop).Path.ToLowerInvariant()
                        [void]$changedSet.Add($res)
                    } catch { }
                }
            }

            # Intersect the scan results with the changed .php set
            $allPaths = $allPaths | Where-Object {
                if (-not (Test-Path -LiteralPath $_ -PathType Leaf)) { return $false }
                try {
                    $rp = (Resolve-Path -LiteralPath $_ -ErrorAction Stop).Path.ToLowerInvariant()
                    $changedSet.Contains($rp)
                } catch {
                    $false
                }
            }
        }
    }

    $now         = Get-Date
    $projectName = Split-Path -Path (Resolve-Path $Path) -Leaf
    $header = @(
        "# $($Config.DumpTitle): $projectName",
        "",
        "**Root:** `$(Resolve-Path $Path)`  ",
        "**Generated:** $($now.ToString('yyyy-MM-dd HH:mm:ss'))  ",
        "**User:** $env:UserDomain\$env:UserName  ",
        "",
        "---",
        ""
    )
    Set-Content -LiteralPath $dumpFile -Value ($header -join "`r`n") -Encoding UTF8

    [int]$written = 0
    [int]$skipped = 0

    foreach ($full in $allPaths) {
        if (-not (Test-Path -LiteralPath $full -PathType Leaf)) {
            $skipped++
            continue
        }

        # Relative-ish name for the heading
        $rel = try {
            Resolve-Path -LiteralPath $full -Relative
        } catch {
            [IO.Path]::GetRelativePath($Path, $full)
        }

        # 1) Extension-based skip for obvious binary stuff
        if (-not $Config.DumpIncludeBinaryLikeExtensions) {
            if ($rel -match $binaryLikePattern) {
                $skipped++
                continue
            }
        }

        # 2) Content-based text check
        $text = Try-ReadText $full
        if ($null -eq $text) {
            $skipped++
            continue
        }

        $lang = Get-CodeFenceLanguage $rel
        $sha1 = Get-FileSha1       $full

        $fi = Get-Item -LiteralPath $full
        $lineCount = if ($text.Length -eq 0) { 0 } else { ($text -split "`r?`n").Count }

        $facts = "**Size:** $($fi.Length) bytes · **Lines:** $lineCount · **Modified:** " +
                 "$($fi.LastWriteTime.ToString('yyyy-MM-dd HH:mm')) · **SHA1:** $sha1"

        $fullResolved = try {
            (Resolve-Path -LiteralPath $full -ErrorAction Stop).Path
        } catch {
            $full
        }

        $fileHeaderLines = @(
            "## $rel",
            "",
            "**Full path:** `$fullResolved`  ",
            $facts,
            ""
        )
        $fileHeader = $fileHeaderLines -join "`r`n"

        Add-Content -LiteralPath $dumpFile -Encoding UTF8 -Value "$fileHeader`r`n```$lang`r`n$text`r`n````r`n"
        $written++
    }

    $footer = @(
        "---",
        "",
        "**Files included:** $written  ",
        "**Files skipped:** $skipped  ",
        ""
    )
    Add-Content -LiteralPath $dumpFile -Value ($footer -join "`r`n")

    "Saved Markdown dossier to: $dumpFile"
}
# === End: Output ===