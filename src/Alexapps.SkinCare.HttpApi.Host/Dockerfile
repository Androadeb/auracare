#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
# Create uploads directory and set permissions as root
RUN mkdir -p /app/wwwroot/uploads && \
    chown -R app:app /app/wwwroot/uploads && \
    chmod -R 775 /app/wwwroot/uploads  # Allow app user to write
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Alexapps.SkinCare.HttpApi.Host/Alexapps.SkinCare.HttpApi.Host.csproj", "src/Alexapps.SkinCare.HttpApi.Host/"]
COPY ["src/Alexapps.SkinCare.Application/Alexapps.SkinCare.Application.csproj", "src/Alexapps.SkinCare.Application/"]
COPY ["src/Alexapps.SkinCare.Domain/Alexapps.SkinCare.Domain.csproj", "src/Alexapps.SkinCare.Domain/"]
COPY ["src/Alexapps.SkinCare.Domain.Shared/Alexapps.SkinCare.Domain.Shared.csproj", "src/Alexapps.SkinCare.Domain.Shared/"]
COPY ["src/Alexapps.SkinCare.Application.Contracts/Alexapps.SkinCare.Application.Contracts.csproj", "src/Alexapps.SkinCare.Application.Contracts/"]
COPY ["src/Alexapps.SkinCare.EntityFrameworkCore/Alexapps.SkinCare.EntityFrameworkCore.csproj", "src/Alexapps.SkinCare.EntityFrameworkCore/"]
COPY ["src/Alexapps.SkinCare.HttpApi/Alexapps.SkinCare.HttpApi.csproj", "src/Alexapps.SkinCare.HttpApi/"]
RUN dotnet restore "./src/Alexapps.SkinCare.HttpApi.Host/Alexapps.SkinCare.HttpApi.Host.csproj"
COPY . .
WORKDIR "/src/src/Alexapps.SkinCare.HttpApi.Host"
RUN dotnet build "./Alexapps.SkinCare.HttpApi.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Alexapps.SkinCare.HttpApi.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Alexapps.SkinCare.HttpApi.Host.dll"]

