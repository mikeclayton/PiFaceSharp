param
(
    [string] $targets = "Build"
)


$ErrorActionPreference = "Stop";
Set-StrictMode -Version "Latest";


$thisScript = $MyInvocation.MyCommand.Path;
$thisFolder = [System.IO.Path]::GetDirectoryName($thisScript);


. ([System.IO.Path]::Combine($thisFolder, "Invoke-MSBuild.ps1"));
. ([System.IO.Path]::Combine($thisFolder, "Invoke-NUnit.ps1"));
. ([System.IO.Path]::Combine($thisFolder, "Read-TeamCityBuildProperties.ps1"));
. ([System.IO.Path]::Combine($thisFolder, "Set-PowerShellHostWidth.ps1"));


Set-PowerShellHostWidth 500;


$properties = Read-TeamCityBuildProperties;


$solution = [System.IO.Path]::Combine($thisFolder, "..\src\Kingsland.PiFaceSharp.sln");
Invoke-MsBuild -solution $solution -targets $targets;


$assembly = [System.IO.Path]::Combine($thisFolder, "..\src\Kingsland.PiFaceSharp.UnitTests\bin\Debug\Kingsland.PiFaceSharp.UnitTests.dll");
Invoke-NUnit -assembly $assembly;
