@echo off

rem v1.0.2.0

rem H is the destination game folder
rem GAMEDIR is the name of the mod folder (usually the mod name)
rem GAMEDATA is the name of the local GameData
rem VERSIONFILE is the name of the version file, usually the same as GAMEDATA,
rem    but not always

rem set directories
set GAMEDIR=OnDemandFuelCells
set GAMEDATA="GameData\"
set H=C:\KSP_DEV\Releases
rem set SUBDIR="KermangeddonIndustries\"
set RELEASEDLL=Source\bin\Release

rem set files
set CHANGELOG="Changelog.cfg"
set README="Readme.*"
set VERSIONFILE=%GAMEDIR%.version
set DLLNAME=%GAMEDIR%.dll

rem set utilities locations
set PD=C:\ProgramData\chocolatey\bin\pandoc.exe

rem create HTML5 version of Readme.md
pandoc -f gfm -t html5 Readme.md -o Readme.htm
rem %PD% -f gfm -t html5 Readme.md -o Readme.htm

rem copy /Y "%1%2" "%GAMEDATA%%GAMEDIR%\Plugins"
rem if automated build - use params, else manual
IF "%~1" == "" (
copy /Y "Source\bin\Release%1%2" "%GAMEDATA%%GAMEDIR%\Plugins"
  ) else (
    copy /Y "%1%2" "%GAMEDATA%\%GAMEDIR%\Plugins"
  )

rem copy /Y "%1%2" "%GAMEDATA%\%GAMEDIR%\Plugins"
copy /Y %VERSIONFILE% %GAMEDATA%\%GAMEDIR%
copy /Y %CHANGELOG% %GAMEDATA%%SUBDIR%%GAMEDIR%
copy /Y %README% %GAMEDATA%%SUBDIR%%GAMEDIR%

xcopy /y /s /I %GAMEDATA%%SUBDIR%%GAMEDIR% "%H%\GameData\%SUBDIR%\%GAMEDIR%"

pause
