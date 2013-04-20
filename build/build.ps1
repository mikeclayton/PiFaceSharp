param($Targets="Build")

$ScriptPath = Split-Path $MyInvocation.MyCommand.Path

$ProjectFile = Join-Path $ScriptPath "..\src\Kingsland.PiFaceSharp.sln"
 
& "$(Get-Content ENV:WINDIR)\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" $ProjectFile /maxcpucount /verbosity:Minimal /target:"$Targets"