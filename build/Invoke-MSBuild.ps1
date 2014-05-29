function Invoke-MsBuild
{

    param
    (

        [Parameter(Mandatory=$true)]
        [string] $solution,

        [Parameter(Mandatory=$true)]
        [string] $targets

    )     

    write-host "--------------";
    write-host "Invoke-MsBuild";
    write-host "--------------";

    $msbuild = "$($env:windir)\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe";
    $version = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($msbuild);

    write-host "msbuild path = $msbuild";
    write-host "msbuild version info = ";
    write-host ($version | fl * | out-string);

    & "$msbuild" $solution /maxcpucount /verbosity:Minimal /target:"$targets"

    if( $LastExitCode -ne 0 )
    {
        throw new-object System.InvalidOperationException("Command failed with exit code $LastExitCode.");
    }

    write-host "--------------";

}