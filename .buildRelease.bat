@echo off

rem v1.0.2.0

rem Put the following text into the Post-build event command line:
rem without the "rem":

rem start /D C:\KSP_DEV\Workspace\ODFCr\ /WAIT deploy.bat  $(TargetDir) $(TargetFileName)
rem
rem if $(ConfigurationName) == Release start /D C:\KSP_DEV\Workspace\ODFCr\ /WAIT buildRelease.bat $(TargetDir) $(TargetFileName)


rem Set variables here

rem H is the destination game folder
rem GAMEDIR is the name of the mod folder (usually the mod name)
rem GAMEDATA is the name of the local GameData
rem VERSIONFILE is the name of the version file, usually the same as GAMEDATA,
rem    but not always
rem LICENSE is the license file
rem README is the readme file

rem set directories
set GAMEDIR=OnDemandFuelCells
set GAMEDATA="GameData\"
rem set SUBDIR="KermangeddonIndustries\"
set RELEASEDIR=C:\KSP_DEV\Releases
set RELEASEDLL=Source\bin\Release

rem set files
Set LICENSETEXT="GPLv3.txt"
rem set LICENSE="License.txt"
set CHANGELOG="Changelog.cfg"
set README="Readme.*"
set DLLNAME=%GAMEDIR%.dll
set VERSIONFILE=%GAMEDIR%.version
set TSLICENSE=""
set TweakScale="Scale_Redist.dll"

rem set utilities locations
set ZIP="c:\Program Files\7-zip\7z.exe"
set JQ=C:\ProgramData\chocolatey\lib\jq\tools\jq.exe

rem Copy files to GameData locations

rem copy /Y "%1%2" "%GAMEDATA%%GAMEDIR%\Plugins"
rem if automated build - use params, else manual
IF "%~1" == "" (
copy /Y "Source\bin\Release%1%2" "%GAMEDATA%%GAMEDIR%\Plugins"
  ) else (
    copy /Y "%1%2" "%GAMEDATA%\%GAMEDIR%\Plugins"
  )

copy /Y %GAMEDIR%.version %GAMEDATA%%SUBDIR%%GAMEDIR%
copy /Y %CHANGELOG% %GAMEDATA%\%SUBDIR%%GAMEDIR%

rem if "%LICENSE%" NEQ "" copy /y  %LICENSE% %GAMEDATA%\%SUBDIR%\%GAMEDIR%
if "%LICENSETEXT%" NEQ "" copy /y  %LICENSETEXT% %GAMEDATA%\%SUBDIR%\%GAMEDIR%
if "%README%" NEQ "" copy /Y %README% %GAMEDATA%\%SUBDIR%\%GAMEDIR%

rem handle TweakScale
if "%TSLICENSE%" NEQ "" copy /y  %TSLICENSE% %GAMEDATA%\%GAMEDIR%\Plugins
if %TweakScale% NEQ "" copy /y %TweakScale% %GAMEDATA%\%GAMEDIR%\Plugins
rem Get Version info

copy %VERSIONFILE% tmp.version
set VERSIONFILE=tmp.version
rem The following requires the JQ program, available here: https://stedolan.github.io/jq/download/
%JQ% ".VERSION.MAJOR" %VERSIONFILE% >tmpfile
set /P major=<tmpfile

%JQ% ".VERSION.MINOR" %VERSIONFILE% >tmpfile
set /P minor=<tmpfile

%JQ% ".VERSION.PATCH" %VERSIONFILE% >tmpfile
set /P patch=<tmpfile

%JQ% ".VERSION.BUILD"  %VERSIONFILE% >tmpfile
set /P build=<tmpfile
del tmpfile
del tmp.version
set VERSION=%major%.%minor%.%patch%.%build%
rem if "%build%" NEQ "0"  set VERSION=%VERSION%.%patch%

echo Version:  %VERSION%


rem Build the zip FILE
cd %GAMEDATA%\..

set FILE="%RELEASEDIR%\%GAMEDIR%-%VERSION%.zip"
IF EXIST %FILE% del /F %FILE%
%ZIP% a -tzip %FILE% GameData

pause
