@echo off

pushd %~dp0

REM Build log clear
echo. > build.log

REM ServerStart
set START_CALL=true
echo START_CALL is set to %START_CALL%

if not exist LocalConfig.txt (
	echo %date% %time% Not exist LocalConfig.txt >> build.log
	echo %date% %time% Set default visuatl studio[2022] >> build.log
	echo "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" > LocalConfig.txt
    exit /b 1
)

set /p MSBuildPath=<LocalConfig.txt
echo MSBuildPath is set to %MSBuildPath%

REM 웹 서버 배치파일 실행
echo call WebServer batch file
call WebServerStart.bat

REM 소켓 서버 배치파일 실행


REM unset parent call
set START_CALL=

pause