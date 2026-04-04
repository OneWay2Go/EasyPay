# Build from the root of the repo to include backend projects
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all project files
COPY ["TgRobuxBot/TgRobuxBot.csproj", "TgRobuxBot/"]
COPY ["backend/EasyPay.Core/EasyPay.Core.csproj", "backend/EasyPay.Core/"]
COPY ["backend/EasyPay.Infrastructure/EasyPay.Infrastructure.csproj", "backend/EasyPay.Infrastructure/"]

# Restore
RUN dotnet restore "TgRobuxBot/TgRobuxBot.csproj"

# Build
COPY . .
RUN dotnet build "TgRobuxBot/TgRobuxBot.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "TgRobuxBot/TgRobuxBot.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TgRobuxBot.dll"]
