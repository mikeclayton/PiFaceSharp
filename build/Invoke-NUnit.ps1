function Invoke-NUnit
{

    param
    (

        [Parameter(Mandatory=$true)]
        [string] $nunitRunnersFolder,

        [Parameter(Mandatory=$true)]
        [string] $assembly

    )     

    write-host "------------";
    write-host "Invoke-NUnit";
    write-host "------------";
    write-host "runners dir = $nunitRunnersFolder";

    $console = [System.IO.Path]::Combine($nunitRunnersFolder, "tools\nunit-console.exe");
    
    write-host "console exe = $console";
    write-host "assembly    = $assembly";

    & "$console" $assembly;

    if( $LastExitCode -ne 0 )
    {
        throw new-object System.InvalidOperationException("Command failed with exit code $LastExitCode.");
    }

    write-host "------------";

}