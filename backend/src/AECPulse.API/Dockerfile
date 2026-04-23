# 1. SDK Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 2. Create the folder structure first
# This ensures Project References in the .csproj files stay valid
RUN mkdir -p backend/src/AECPulse.API/
RUN mkdir -p backend/src/AECPulse.Infrastructure/
RUN mkdir -p backend/src/AECPulse.Application/

# 3. Copy only the .csproj files first (for efficient caching)
COPY ["backend/src/AECPulse.API/AECPulse.API.csproj", "backend/src/AECPulse.API/"]
COPY ["backend/src/AECPulse.Infrastructure/AECPulse.Infrastructure.csproj", "backend/src/AECPulse.Infrastructure/"]
COPY ["backend/src/AECPulse.Application/AECPulse.Application.csproj", "backend/src/AECPulse.Application/"]

# 4. Restore using the API project as the entry point
# We use the full path so the SDK can find the references in the subfolders
RUN dotnet restore "backend/src/AECPulse.API/AECPulse.API.csproj"

# 5. Copy everything else and build
COPY . .
WORKDIR "/src/backend/src/AECPulse.API"
RUN dotnet publish "AECPulse.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 6. Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AECPulse.API.dll"]