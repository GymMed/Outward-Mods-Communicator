@echo off
setlocal

REM === Step 1: Placing dll document to game ===
echo Placing dll document to game...
call "%~dp0placeBuild.bat"

REM === Step 2: Run copy script ===
call "%~dp0placeLatestNuget.bat"

echo Done!
pause