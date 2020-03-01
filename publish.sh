#!/usr/bin/env bash
export NUGET_SERVER=https://api.nuget.org/v3/index.json
rm -rf src/EventBus/bin/Release
rm -rf src/EventBus.DependencyInjection/bin/Release

dotnet build -c Release
dotnet pack -c Release

nuget push src/EventBus/bin/Release/*.nupkg -SkipDuplicate -Source $NUGET_SERVER
nuget push src/EventBus.DependencyInjection/bin/Release/*.nupkg -SkipDuplicate  -Source $NUGET_SERVER
nuget push src/EventBus.RabbitMQ/bin/Release/*.nupkg -SkipDuplicate -Source $NUGET_SERVER
nuget push src/EventBus.RabbitMQ.DependencyInjection/bin/Release/*.nupkg -SkipDuplicate  -Source $NUGET_SERVER

sudo cp src/EventBus/bin/Release/*.nupkg  /usr/local/share/dotnet/sdk/NuGetFallbackFolder
sudo cp src/EventBus.DependencyInjection/bin/Release/*.nupkg  /usr/local/share/dotnet/sdk/NuGetFallbackFolder
sudo cp src/EventBus.RabbitMQ/bin/Release/*.nupkg  /usr/local/share/dotnet/sdk/NuGetFallbackFolder
sudo cp src/EventBus.RabbitMQ.DependencyInjection/bin/Release/*.nupkg  /usr/local/share/dotnet/sdk/NuGetFallbackFolder