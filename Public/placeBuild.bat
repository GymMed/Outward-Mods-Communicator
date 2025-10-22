@echo off
setlocal enabledelayedexpansion

:: Source paths
set "sourceDll=C:\Users\pc\source\repos\OutwardModsCommunicator\Release\OutwardModsCommunicator.dll"

:: Profiles array (quoted entries for readability)
set profiles="Gytis" "Development"

:: Base destination folder
set "baseProfilePath=C:\Users\pc\AppData\Roaming\r2modmanPlus-local\OutwardDe\profiles"

:: --- Copy DLL into each profile ---
if exist "%sourceDll%" (
    for %%p in (%profiles%) do (
        set "destinationDll=%baseProfilePath%\%%~p\BepInEx\plugins\gymmed-OutwardModsCommunicator"
        echo Copying "%sourceDll%" to "!destinationDll!"
        if not exist "!destinationDll!" mkdir "!destinationDll!"
        copy /Y "%sourceDll%" "!destinationDll!"
    )
) else (
    echo Source dll file does not exist!
)

pause