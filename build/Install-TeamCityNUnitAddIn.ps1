function Install-TeamCityNUnitAddIn()
{

    param
    (

        [Parameter(Mandatory=$true)]
        [string] $teamcityAddinPath,

        [Parameter(Mandatory=$true)]
        [string] $nunitRunnersFolder

    )

    write-host "--------------------------";
    write-host "Install-TeamCityNUnitAddIn";
    write-host "--------------------------";
    write-host "addin path = $teamcityAddinPath";
    write-host "runner dir = $nunitRunnersFolder";

    $root = [System.IO.Path]::GetDirectoryName($teamCityAddinPath);
    if( [System.IO.Directory]::Exists($root) )
    {
        foreach( $file in [System.IO.Directory]::GetFiles($root) )
        {
            write-host $file;
        }
    }

    if( -not [System.IO.Directory]::Exists($teamcityAddinPath) )
    {
        write-host "addin path does not exist";
        return;
    }

    throw new-object System.NotImplementedException;

    write-host "copying directories = ";
    foreach( $folder in [System.IO.Directory]::GetDirectories($teamcityAddinPath) )
    {
        write-host $folder;
    }

    write-host "copying files = ";
    foreach( $file in [System.IO.Directory]::GetFiles($teamcityAddinPath) )
    {
        write-host $file;
    }

    write-host "--------------------------";

}