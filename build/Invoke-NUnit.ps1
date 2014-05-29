function Invoke-NUnit
{

    param
    (

        [Parameter(Mandatory=$true)]
        [string] $assembly

    )     

    write-host "----------------";
    write-host "Invoke-UnitTests";
    write-host "----------------";

    $nunit = [System.IO.Path]::Combine($thisFolder, "..\src\packages\NUnit.Runners.2.6.3\tools\nunit-console.exe");
    
    write-host "nunit path = $nunit";
    write-host "assembly   = $assembly";

    & "$nunit" $assembly;

    if( $LastExitCode -ne 0 )
    {
        throw new-object System.InvalidOperationException("Command failed with exit code $LastExitCode.");
    }

    write-host "----------------";

}