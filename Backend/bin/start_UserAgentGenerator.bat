@echo off

cd %~dp0
cd ..
set BASE=%CD%
set CLASSPATH="%BASE%/lib/*"

if "%~2"=="" (
    echo Syntax error!
	echo.
    echo Usage: start_UserAgentGenerator user.name user.password user.mail
) else (
	java -cp %CLASSPATH% i5.las2peer.tools.UserAgentGenerator %2 %1 %3
)