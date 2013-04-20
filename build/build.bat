@echo Off

set powershell=%windir%\System32\WindowsPowerShell\v1.0\powershell.exe
set dir=%~dp0%

"%powershell%" -NoProfile -ExecutionPolicy unrestricted -Command "& '%dir%Build.ps1' %*"