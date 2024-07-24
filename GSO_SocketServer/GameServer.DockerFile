# Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy the project file and restore any dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the application code and build it
COPY . ./
RUN dotnet publish -c Release -o out

# Create the runtime image
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app

# Copy the built application from the build stage
COPY --from=build-env /app/out .

# Expose ports (use the specific ports your game server uses)
EXPOSE 7000-8000

# Define the entry point for the container
ENTRYPOINT ["dotnet", "GSO_SocketServer.dll"]