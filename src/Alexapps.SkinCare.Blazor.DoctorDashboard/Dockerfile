FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the project files
COPY ["src/Alexapps.SkinCare.Blazor.DoctorDashboard/Alexapps.SkinCare.Blazor.DoctorDashboard.csproj", "src/Alexapps.SkinCare.Blazor.DoctorDashboard/"]
RUN dotnet restore "./src/Alexapps.SkinCare.Blazor.DoctorDashboard/Alexapps.SkinCare.Blazor.DoctorDashboard.csproj"

# Copy everything else
COPY . .

WORKDIR "/src/src/Alexapps.SkinCare.Blazor.DoctorDashboard"

# Build and publish .NET app
RUN dotnet build "./Alexapps.SkinCare.Blazor.DoctorDashboard.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet publish "./Alexapps.SkinCare.Blazor.DoctorDashboard.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ynex.dll"]
