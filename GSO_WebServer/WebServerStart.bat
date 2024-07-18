@echo off

REM 주의
REM GSO_StartUp.bat을 실행시켜주세요
REM 직접적으로 먼저 실행이 불가능합니다

echo WebServer batch file is running.

set Configuration=Debug

if %Configuration%==Debug (
	set SolutionPath="%~dp0GSO_WebServer.sln"
	set GsoWebServerPath="%~dp0GsoWebServer\bin\Debug\net8.0\"
	set MatchmakingPath="%~dp0MatchmakingServer\bin\Debug\net8.0\"
	set GameSessionPath="%~dp0GameSessionServer\bin\Debug\net8.0\"
) else if %Configuration%==Release (
	set SolutionPath="%~dp0GSO_WebServer.sln"
	set GsoWebServerPath="%~dp0GsoWebServer\bin\Release\net8.0\"
	set MatchmakingPath="%~dp0MatchmakingServer\bin\Release\net8.0\"
	set GameSessionPath="%~dp0GameSessionServer\bin\Release\net8.0\"
)

if %START_CALL%==true (

	REM GSO_WebServer 솔루션 빌드 시작
	echo build to GSO_WebServer solution.
	%MSBuildPath% %SolutionPath% /p:Configuration=%Configuration%
	
	if %ERRORLEVEL% NEQ 0 (
		echo GSO_WebServer Build failed.
		echo %date% %time% Build failed with error code %ERRORLEVEL% >> build.log
		exit /b %ERRORLEVEL%
	)

	REM 인증 서버 시작
	echo start to GsoWebServer.
	start /d %GsoWebServerPath% GsoWebServer.exe

	REM 매칭 서버
	REM echo start to MatchmakingServer.
	REM start /d %GsoWebServerPath% MatchmakingServer.exe
	
	REM 게임 세션 서버
	REM echo start to GameSessionServer.
	REM start /d %GameSessionPath% GameSessionServer.exe
	
) else (
	echo This batch file cannot be run directly.
	echo %date% %time% WebServer batch file cannot be run directly. Please Run GSO_StartUp fist >> build.log
    exit /b 1 
)