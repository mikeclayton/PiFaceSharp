param
(
    [string] $targets = "Build"
)


$ErrorActionPreference = "Stop";
Set-StrictMode -Version "Latest";


$thisScript = $MyInvocation.MyCommand.Path;
$thisFolder = [System.IO.Path]::GetDirectoryName($thisScript);


$msbuild = "$($env:windir)\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe";
$version = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($msbuild);


write-host "--------------------";
write-host "msbuild version info";
write-host "--------------------";
write-host ($version | fl * | out-string);
write-host "--------------------";


$solution = [System.IO.Path]::Combine($thisFolder, "..\src\Kingsland.PiFaceSharp.sln");


& "$msbuild" $solution /maxcpucount /verbosity:Minimal /target:"$targets"