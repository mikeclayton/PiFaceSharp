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


$solution     = [System.IO.Path]::Combine($thisFolder, "..\src\Kingsland.PiFaceSharp.sln");
$assembly     = [System.IO.Path]::Combine($thisFolder, "..\src\Kingsland.PiFaceSharp.UnitTests\bin\Debug\Kingsland.PiFaceSharp.UnitTests.dll");
$nunitRunners = [System.IO.Path]::Combine($thisFolder, "..\src\packages\NUnit.Runners.2.6.3");


$properties = Read-TeamCityBuildProperties;
if( $properties -ne $null )
{
    Install-TeamCityNUnitAddIn -teamcityAddinPath $properties["teamcity.dotnet.nunitaddin"] `
                               -nunitRunnersFolder $nunitRunners;
}


Invoke-MsBuild -solution $solution -targets $targets;


Invoke-NUnit -nunitRunnersFolder $nunitRunners `
             -assembly $assembly;
