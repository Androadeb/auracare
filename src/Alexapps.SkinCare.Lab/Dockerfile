FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Install Node.js
RUN curl -fsSL https://deb.nodesource.com/setup_20.x | bash - && \
    apt-get install -y nodejs

# Copy the project files
COPY ["src/Alexapps.SkinCare.Lab/starterkit.csproj", "src/Alexapps.SkinCare.Lab/"]
RUN dotnet restore "./src/Alexapps.SkinCare.Lab/starterkit.csproj"

# Copy everything else
COPY . .

# Build JS assets
WORKDIR "/src/src/Alexapps.SkinCare.Lab"
RUN npm install
RUN npm run ynex

# Build and publish .NET app
RUN dotnet build "./starterkit.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet publish "./starterkit.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "starterkit.dll"]
