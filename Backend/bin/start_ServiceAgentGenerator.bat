@echo off

cd %~dp0
cd ..
set BASE=%CD%
set CLASSPATH="%BASE%/lib/*;"

if "%~2"=="" (
    echo Syntax error!
	echo. 
    echo Usage: start_ServiceAgentGenerator service.canonical.class.name service.password
) else (
	java -cp %CLASSPATH% i5.las2peer.tools.ServiceAgentGenerator %1 %2
)
