FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src

COPY "Services/IdentityControl/" "Services/IdentityControl/"
COPY "MercuryBus" "MercuryBus"

RUN dotnet publish Services/IdentityControl/IdentityControl.API/IdentityControl.API.csproj -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "IdentityControl.API.dll"]
