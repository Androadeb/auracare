#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Alexapps.SkinCare.DbMigrator/Alexapps.SkinCare.DbMigrator.csproj", "src/Alexapps.SkinCare.DbMigrator/"]
COPY ["src/Alexapps.SkinCare.Application.Contracts/Alexapps.SkinCare.Application.Contracts.csproj", "src/Alexapps.SkinCare.Application.Contracts/"]
COPY ["src/Alexapps.SkinCare.Domain.Shared/Alexapps.SkinCare.Domain.Shared.csproj", "src/Alexapps.SkinCare.Domain.Shared/"]
COPY ["src/Alexapps.SkinCare.EntityFrameworkCore/Alexapps.SkinCare.EntityFrameworkCore.csproj", "src/Alexapps.SkinCare.EntityFrameworkCore/"]
COPY ["src/Alexapps.SkinCare.Domain/Alexapps.SkinCare.Domain.csproj", "src/Alexapps.SkinCare.Domain/"]
RUN dotnet restore "./src/Alexapps.SkinCare.DbMigrator/Alexapps.SkinCare.DbMigrator.csproj"
COPY . .
WORKDIR "/src/src/Alexapps.SkinCare.DbMigrator"
RUN dotnet build "./Alexapps.SkinCare.DbMigrator.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Alexapps.SkinCare.DbMigrator.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Alexapps.SkinCare.DbMigrator.dll"]
