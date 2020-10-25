@ECHO OFF
REM publish CLI - call scripts/publish-cli.bat
dotnet publish Kanpachi.CLI -c Release -p:PublishSingleFile=true:PublishTrimmed=true -o Kanpachi.Release
copy scripts\kanpachi.bat Kanpachi.Release\kanpachi.bat