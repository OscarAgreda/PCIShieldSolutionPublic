# Typical use (recommended)
# .\Export-ProjectMarkdown.ps1 -Root 'C:\_tj\_a_php_crm' -OutFile '.\PROJECT_DUMP.md'

# Include every file even if it looks binary (not recommended; can bloat)
# .\Export-ProjectMarkdown.ps1 -Root 'C:\_tj\_a_php_crm' -OutFile '.\PROJECT_DUMP.md' -IncludeBinaryLikeExtensions




#requires -Version 5.1
param(
  [Parameter(Mandatory=$false)]
  [string]$Root = (Get-Location).Path,

  [Parameter(Mandatory=$false)]
  [string]$OutFile = ".\PROJECT_DUMP.md",

  # If set, includes files with typical binary extensions (NOT recommended)
  [switch]$IncludeBinaryLikeExtensions
)

$ErrorActionPreference = 'Stop'

function Write-Info($msg)  { Write-Host "[INFO ] $msg" -ForegroundColor Cyan }
function Write-Warn($msg)  { Write-Host "[WARN ] $msg" -ForegroundColor Yellow }
function Write-Err ($msg)  { Write-Host "[ERROR] $msg" -ForegroundColor Red }

# --- Validate root and git
if (-not (Test-Path $Root)) { Write-Err "Root path not found: $Root"; exit 1 }
Set-Location -Path $Root

$git = Get-Command git -ErrorAction SilentlyContinue
if (-not $git) { Write-Err "git not found on PATH. Please install Git."; exit 1 }

# --- Gather file list from git
Write-Info "Collecting git-tracked files…"
$files = & git ls-files
if (-not $files) { Write-Err "No files returned by 'git ls-files' under $Root."; exit 1 }

# --- Extensions to skip (typical binaries / heavy assets)
$binaryLikePattern = '(?i)\.(png|jpe?g|gif|bmp|ico|md|webp|svgz?|tiff?|mp3|wav|ogg|mp4|m4v|mov|avi|wmv|zip|7z|rar|gz|bz2|xz|pdf|docx?|xlsx?|pptx?|exe|dll|pdb|so|dylib|bin|iso|lock)$'

# --- Language map for code fences
$languageMap = @{
  '.php'   = 'php'
  '.phtml' = 'php'
  '.html'  = 'html'
  '.htm'   = 'html'
  '.htaccess' = 'apache'
  '.css'   = 'css'
  '.js'    = 'javascript'
  '.ts'    = 'typescript'
  '.json'  = 'json'
  '.xml'   = 'xml'
  '.yml'   = 'yaml'
  '.yaml'  = 'yaml'
  '.sql'   = 'sql'
  '.md'    = 'markdown'
  '.mdx'   = 'markdown'
  '.txt'   = 'text'
  '.ini'   = 'ini'
  '.conf'  = 'ini'
  '.env'   = 'ini'
  '.csv'   = 'csv'
  '.twig'  = 'twig'
  '.vue'   = 'vue'
  '.sh'    = 'bash'
  '.ps1'   = 'powershell'
  '.bat'   = 'batch'
  '.lock'  = 'text'
}

function Get-CodeFenceLanguage([string]$path) {
  $ext = [System.IO.Path]::GetExtension($path)
  if ($languageMap.ContainsKey($ext)) { return $languageMap[$ext] }
  # Special cases by name
  $name = [System.IO.Path]::GetFileName($path)
  if ($name -eq '.htaccess') { return 'apache' }
  return 'text'
}

# Utility: SHA1 for quick integrity hint
function Get-FileSha1([string]$fullPath) {
  try {
    $sha = [System.Security.Cryptography.SHA1]::Create()
    $fs = [System.IO.File]::OpenRead($fullPath)
    try {
      $hashBytes = $sha.ComputeHash($fs)
      return ($hashBytes | ForEach-Object { $_.ToString('x2') }) -join ''
    } finally {
      $fs.Dispose()
      $sha.Dispose()
    }
  } catch {
    return ''
  }
}

# Utility: read text safely; return $null if binary-ish / unreadable
function Try-ReadText([string]$fullPath) {
  try {
    # Fast path: UTF8 read
    return Get-Content -LiteralPath $fullPath -Raw -Encoding UTF8
  } catch {
    try {
      # Fallback to default encoding
      return Get-Content -LiteralPath $fullPath -Raw
    } catch {
      return $null
    }
  }
}

# --- Prepare header
$now = Get-Date
$projectName = Split-Path -Path (Resolve-Path $Root) -Leaf
$header = @()
$header += "# Project Dossier: $projectName"
$header += ""
$header += "**Root:** `$(Resolve-Path $Root)`  "
$header += "**Generated:** $($now.ToString('yyyy-MM-dd HH:mm:ss'))  "
$header += "**User:** $env:UserDomain\$env:UserName  "
$header += ""
$header += "---"
$header += ""
Set-Content -LiteralPath $OutFile -Value ($header -join "`r`n") -Encoding UTF8

# --- Process files
[int]$writtenCount = 0
[int]$skippedCount = 0

foreach ($relPath in $files) {
  $fullPath = Join-Path -Path $Root -ChildPath $relPath

  if (-not (Test-Path -LiteralPath $fullPath)) {
    Write-Warn "Missing on disk (skipped): $relPath"
    $skippedCount++
    continue
  }

  # Skip binary-ish by extension unless explicitly allowed
  if (-not $IncludeBinaryLikeExtensions) {
    if ($relPath -match $binaryLikePattern) {
      Write-Info "Skipping binary-like: $relPath"
      $skippedCount++
      continue
    }
  }

  $fi = Get-Item -LiteralPath $fullPath
  $lang = Get-CodeFenceLanguage $relPath
  $sha1 = Get-FileSha1 $fullPath

  $text = Try-ReadText $fullPath
  if ($null -eq $text) {
    Write-Warn "Unreadable text (skipped): $relPath"
    $skippedCount++
    continue
  }

  $lineCount = if ($text.Length -eq 0) { 0 } else { ($text -split "`r?`n").Count }
  $facts = "**Size:** $($fi.Length) bytes · **Lines:** $lineCount · **Modified:** $($fi.LastWriteTime.ToString('yyyy-MM-dd HH:mm')) · **SHA1:** $sha1"

  # Write section
  Add-Content -LiteralPath $OutFile -Encoding UTF8 -Value "## $relPath`r`n$facts`r`n```$lang`r`n$text`r`n````r`n"
  $writtenCount++
}

# --- Footer summary
$footer = @()
$footer += "---"
$footer += ""
$footer += "**Files included:** $writtenCount  "
$footer += "**Files skipped:** $skippedCount  "
$footer += ""
Add-Content -LiteralPath $OutFile -Encoding UTF8 -Value ($footer -join "`r`n")

Write-Info "Markdown written to: $(Resolve-Path $OutFile)"
