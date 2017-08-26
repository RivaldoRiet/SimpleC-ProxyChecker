cd /d "c:\users\rivaldo\documents\visual studio 2015\Projects\proxychecker\proxychecker" &msbuild "proxychecker.csproj" /t:sdvViewer /p:configuration="Debug" /p:platform=Any CPU
exit %errorlevel% 