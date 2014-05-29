param
(
    $targets = "Build"
)


$thisScript = $MyInvocation.MyCommand.Path;
$thisFolder = [System.IO.Path]::GetDirectoryName($thisScript);


$msbuild = "$($env:windir)\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe";
$solution = [System.IO.Path]::Combine($thisFolder, "..\src\Kingsland.PiFaceSharp.sln");


& "$msbuild" $solution /maxcpucount /verbosity:Minimal /target:"$targets"