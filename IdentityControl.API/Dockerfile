#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src

COPY "Brevien.sln" "Brevien.sln"

COPY "EventBus/EventBus/EventBus.csproj" "EventBus/EventBus/EventBus.csproj"
COPY "EventBus/EventBusRabbitMQ/EventBusRabbitMQ.csproj" "EventBus/EventBusRabbitMQ/EventBusRabbitMQ.csproj"
COPY "EventBus/IntegrationEventLog/IntegrationEventLog.csproj" "EventBus/IntegrationEventLog/IntegrationEventLog.csproj"
COPY "Services/Blogging/Blogging.API/Blogging.API.csproj" "Services/Blogging/Blogging.API/Blogging.API.csproj"
COPY "Services/Blogging/Blogging.IntegrationTests/Blogging.IntegrationTests.csproj" "Services/Blogging/Blogging.IntegrationTests/Blogging.IntegrationTests.csproj"
COPY "Services/IdentityControl/IdentityControl.API/IdentityControl.API.csproj" "Services/IdentityControl/IdentityControl.API/IdentityControl.API.csproj"
COPY "Services/IdentityServer/IdentityServer.API/IdentityServer.API.csproj" "Services/IdentityServer/IdentityServer.API/IdentityServer.API.csproj"
COPY "Services/Posting/Posting.API/Posting.API.csproj" "Services/Posting/Posting.API/Posting.API.csproj"
COPY "Services/Posting/Posting.Core/Posting.Core.csproj" "Services/Posting/Posting.Core/Posting.Core.csproj"
COPY "Services/Posting/Posting.Infrastructure/Posting.Infrastructure.csproj" "Services/Posting/Posting.Infrastructure/Posting.Infrastructure.csproj"
COPY "Services/Posting/Posting.IntegrationTests/Posting.IntegrationTests.csproj" "Services/Posting/Posting.IntegrationTests/Posting.IntegrationTests.csproj"
COPY "Services/Posting/Posting.UnitTests/Posting.UnitTests.csproj" "Services/Posting/Posting.UnitTests/Posting.UnitTests.csproj"
COPY "docker-compose.dcproj" "docker-compose.dcproj"

RUN dotnet restore "Brevien.sln"

COPY . .
WORKDIR "/src/Services/IdentityControl/IdentityControl.API"
RUN dotnet publish --no-restore -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "IdentityControl.API.dll"]
