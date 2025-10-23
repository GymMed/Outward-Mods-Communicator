@echo off
setlocal

set "sourceDir=C:\Users\pc\source\repos\OutwardModsCommunicator\Release"
set "destDir=C:\Users\pc\source\repos\LocalNuget"

echo Searching for latest .nupkg in "%sourceDir%"...

REM Find the newest .nupkg file
for /f "delims=" %%F in ('dir "%sourceDir%\*.nupkg" /b /o-d') do (
    set "latest=%%F"
    goto :found
)

echo No .nupkg file found!
goto :eof

:found
echo Found latest package: %latest%
echo Copying to "%destDir%"...

if not exist "%destDir%" mkdir "%destDir%"
copy "%sourceDir%\%latest%" "%destDir%\%latest%" /Y

echo Successfully copied "%latest%" to "%destDir%"
endlocal