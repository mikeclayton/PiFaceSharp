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

    write-host "directories = ";
    foreach( $folder in [System.IO.Directory]::GetDirectories($teamcityAddinPath) )
    {
        write-host $folder;
    }

    write-host "files = ";
    foreach( $file in [System.IO.Directory]::GetFiles($teamcityAddinPath) )
    {
        write-host $file;
    }

    write-host "--------------------------";

}