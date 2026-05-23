# Repara ML.NET Model Builder en Visual Studio 2026 (DLL CodeAnalysis en AutoMLService).
# Usa la MISMA version 5.0.0 en todos los ensamblados (evita mezclar 4.9 + 5.0).
# Ejecutar PowerShell COMO ADMINISTRADOR desde la raiz del repo:
#   .\scripts\fix-model-builder-vs2026.ps1

$ErrorActionPreference = "Stop"
Add-Type -AssemblyName System.IO.Compression.FileSystem

function Get-DllFromNuGetNupkg {
    param(
        [string]$PackageId,
        [string]$DllFileName,
        [string]$Version = "5.0.0"
    )

    $nugetRoot = (& dotnet nuget locals global-packages -l).Split(" ", 2)[1].Trim()
    $pkgDir = Join-Path $nugetRoot "$PackageId\$Version"
    $nupkg = Get-ChildItem (Join-Path $pkgDir "*.nupkg") -ErrorAction SilentlyContinue | Select-Object -First 1

    if (-not $nupkg) {
        Write-Host "Descargando $PackageId $Version..."
        dotnet nuget install $PackageId -Version $Version --no-cache 2>&1 | Out-Null
        $nupkg = Get-ChildItem (Join-Path $pkgDir "*.nupkg") | Select-Object -First 1
    }
    if (-not $nupkg) { throw "No hay paquete $PackageId $Version" }

    $extractDir = Join-Path $env:TEMP "rs-nupkg-$PackageId-$Version"
    if (Test-Path $extractDir) { Remove-Item $extractDir -Recurse -Force }
    New-Item -ItemType Directory -Path $extractDir | Out-Null
    [System.IO.Compression.ZipFile]::ExtractToDirectory($nupkg.FullName, $extractDir)

    $dll = Get-ChildItem $extractDir -Recurse -Filter $DllFileName |
        Where-Object { $_.FullName -match '\\lib\\netstandard2\.0\\' } |
        Select-Object -First 1
    if (-not $dll) {
        $dll = Get-ChildItem $extractDir -Recurse -Filter $DllFileName | Select-Object -First 1
    }
    if (-not $dll) { throw "No se encontro $DllFileName en $($nupkg.FullName)" }
    return $dll.FullName
}

Write-Host "Buscando DLLs CodeAnalysis 5.0.0..."

$files = @(
    @{ Package = "microsoft.codeanalysis.csharp"; Dll = "Microsoft.CodeAnalysis.CSharp.dll" },
    @{ Package = "microsoft.codeanalysis.common"; Dll = "Microsoft.CodeAnalysis.dll" },
    @{ Package = "microsoft.codeanalysis.workspaces.common"; Dll = "Microsoft.CodeAnalysis.Workspaces.dll" }
)

$vsRoots = @(
    "${env:ProgramFiles}\Microsoft Visual Studio\18\Community",
    "${env:ProgramFiles}\Microsoft Visual Studio\2022\Community"
)

$targetSub = "Common7\IDE\CommonExtensions\Microsoft\ModelBuilder\AutoMLService"

foreach ($vs in $vsRoots) {
    $dest = Join-Path $vs $targetSub
    if (-not (Test-Path $dest)) { continue }

    Write-Host "Copiando a: $dest"
    foreach ($f in $files) {
        $src = Get-DllFromNuGetNupkg $f.Package $f.Dll "5.0.0"
        Copy-Item $src $dest -Force
        Write-Host "  OK $($f.Dll)"
    }
    Write-Host "Listo (todo 5.0.0). Cierra Visual Studio, abrelo y reintenta o usa Siguiente si el modelo ya entreno."
    exit 0
}

Write-Error "No se encontro Model Builder (AutoMLService)."
