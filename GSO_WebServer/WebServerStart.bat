@echo off

REM ����
REM GSO_StartUp.bat�� ��������ּ���
REM ���������� ���� ������ �Ұ����մϴ�

echo WebServer batch file is running.

if %Configuration%==Debug (
	set SolutionPath="%~dp0GSO_WebServer.sln"
	set GsoWebServerPath="%~dp0GsoWebServer\bin\Debug\net8.0\"
	set MatchmakerPath="%~dp0MatchmakerPath\bin\Debug\net8.0\"
	set GameServerManagerPath="%~dp0GameServerManager\bin\Debug\net8.0\"
) else if %Configuration%==Release (
	set SolutionPath="%~dp0GSO_WebServer.sln"
	set GsoWebServerPath="%~dp0GsoWebServer\bin\Release\net8.0\"
	set MatchmakerPath="%~dp0atchmakerPath\bin\Release\net8.0\"
	set GameServerManagerPath="%~dp0GameServerManager\bin\Release\net8.0\"
)

if %START_CALL%==true (

	REM GSO_WebServer �ַ�� ���� ����
	echo build to GSO_WebServer solution.
	%MSBuildPath% %SolutionPath% /p:Configuration=%Configuration%
	
	if %ERRORLEVEL% NEQ 0 (
		echo GSO_WebServer Build failed.
		echo %date% %time% Build failed with error code %ERRORLEVEL% >> build.log
		exit /b %ERRORLEVEL%
	)

	REM ���� ���� ����
	echo start to GsoWebServer.
	start /d %GsoWebServerPath% GsoWebServer.exe

	REM ��Ī ����
	echo start to MatchmakingServer.
	start /d %MatchmakerPath% MatchmakingServer.exe
	
	REM ���� ���� ����
	echo start to GameSessionServer.
	start /d %GameServerManagerPath% GameSessionServer.exe
	
) else (
	echo This batch file cannot be run directly.
	echo %date% %time% WebServer batch file cannot be run directly. Please Run GSO_StartUp fist >> build.log
    exit /b 1 
)