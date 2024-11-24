dotnet publish -r win-x64 -c Release /p:PublishSingleFile=true /p:PublishTrimmed=true /p:DebugType=None
dotnet publish -r linux-x64 -c Release /p:PublishSingleFile=true /p:PublishTrimmed=true /p:DebugType=None
dotnet publish -r osx-x64 -c Release /p:PublishSingleFile=true /p:PublishTrimmed=true /p:DebugType=None
