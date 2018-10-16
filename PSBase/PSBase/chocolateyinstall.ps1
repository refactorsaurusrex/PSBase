$ErrorActionPreference = 'Stop'
$appDataFolder = "$env:LocalAppData\$env:ChocolateyPackageName\$env:ChocolateyPackageVersion"
if (-not (Test-Path $appDataFolder)) {
  New-Item $appDataFolder -ItemType Directory
}

Get-ChildItem -Path "$env:ChocolateyPackageFolder" -Include "*.dll","*.ps1","*.psm1","*.psd1","*.xml","*.config" -Recurse | Copy-Item -Destination $appDataFolder 
Write-Debug "Copied contents of '$env:ChocolateyPackageFolder' to '$appDataFolder'"

$psModuleFolder = "$home\Documents\WindowsPowerShell\Modules\$env:ChocolateyPackageName"

if (-not (Test-Path $psModuleFolder)) {
  New-item $psModuleFolder -ItemType Directory
  Write-Debug "Created new module directory: '$psModuleFolder'"
} 

$module = Get-Item -Path "$env:ChocolateyPackageFolder\$env:ChocolateyPackageName.psm1"
Copy-Item -Path $module -Destination $psModuleFolder -Force
Write-Debug "Copied '$module' to '$psModuleFolder'"

$originalManifest = Get-Item -Path "$env:ChocolateyPackageFolder\$env:ChocolateyPackageName.psd1"
Copy-Item -Path $originalManifest -Destination $psModuleFolder -Force
Write-Debug "Copied '$originalManifest' to '$psModuleFolder'"

$rootModulePath = "$appDataFolder\$env:ChocolateyPackageName.dll"
$deployedManifestPath = "$psModuleFolder\$env:ChocolateyPackageName.psd1"
(Get-Content $deployedManifestPath).Replace('$RootModule$', $rootModulePath).Replace('$version$', "$env:ChocolateyPackageVersion") | Set-Content $deployedManifestPath

$oldInstalls = Get-ChildItem -Path "$env:LocalAppData\$env:ChocolateyPackageName" -Directory | 
  Sort-Object -Descending -Property @{Expression={([Version]$_.Name)};} |
  Select-Object -Skip 2

if ($null -ne $oldInstalls) {
  $oldInstalls | ForEach-Object { Write-Debug "Attempting to remove old install: $($_.FullName)" }
  $oldInstalls | Remove-Item -Recurse -Force -ErrorAction Ignore
}