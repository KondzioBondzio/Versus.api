FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Versus.Host/Versus.Host.csproj", "src/Versus.Host/"]
COPY ["src/Versus.Api/Versus.Api.csproj", "src/Versus.Api/"]
COPY ["src/Versus.Core/Versus.Core.csproj", "src/Versus.Core/"]
COPY ["src/Versus.Domain/Versus.Domain.csproj", "src/Versus.Domain/"]
RUN dotnet restore "src/Versus.Host/Versus.Host.csproj"
COPY . .
WORKDIR "/src/src/Versus.Host"
RUN dotnet build "Versus.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Versus.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Versus.Host.dll"]
