#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src

COPY "Brevien.sln" "Brevien.sln"

COPY "Services/Blog/Blog.API/Blog.API.csproj" "Services/Blog/Blog.API/Blog.API.csproj"
COPY "Services/Blog/Blog.FunctionalTests/Blog.FunctionalTests.csproj" "Services/Blog/Blog.FunctionalTests/Blog.FunctionalTests.csproj"
COPY "Services/IdentityControl/IdentityControl.API/IdentityControl.API.csproj" "Services/IdentityControl/IdentityControl.API/IdentityControl.API.csproj"
COPY "Services/IdentityServer/IdentityServer.API/IdentityServer.API.csproj" "Services/IdentityServer/IdentityServer.API/IdentityServer.API.csproj"

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
