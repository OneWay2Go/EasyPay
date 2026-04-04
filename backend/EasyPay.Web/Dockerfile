# Use the .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ["EasyPay.Web/EasyPay.Web.csproj", "EasyPay.Web/"]
COPY ["EasyPay.Core/EasyPay.Core.csproj", "EasyPay.Core/"]
COPY ["EasyPay.Application/EasyPay.Application.csproj", "EasyPay.Application/"]
COPY ["EasyPay.Infrastructure/EasyPay.Infrastructure.csproj", "EasyPay.Infrastructure/"]
RUN dotnet restore "EasyPay.Web/EasyPay.Web.csproj"

# Copy everything else and build
COPY . .
RUN dotnet build "EasyPay.Web/EasyPay.Web.csproj" -c Release -o /app/build

# Publish the app
FROM build AS publish
RUN dotnet publish "EasyPay.Web/EasyPay.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EasyPay.Web.dll"]
