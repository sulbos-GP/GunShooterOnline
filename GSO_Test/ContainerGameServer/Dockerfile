# Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory
WORKDIR /app

# Copy the project files to the container
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the application code
COPY . ./

# Build the application
RUN dotnet publish -c Release -o out

# Use the official .NET Runtime image to run the application
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime

# Set the working directory
WORKDIR /app

# Copy the build output from the previous stage
COPY --from=build /app/out .

# Define the command to run the application
ENTRYPOINT ["dotnet", "ContainerGameServer.dll"]