# First stage: Build & Publish the App
# Use the SDK image to compile the project code
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
WORKDIR /app

# Copy the .csproj and restore dependencies (install libraries)
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build the release
COPY . ./
RUN dotnet publish -c Release -o out

# Stage 2: Runtime
# Use the small ASP.NET Core runtime image to run the production compiled code
## This stage becomes your final image, not the first stage as it's abandoned when it has completed.
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Copy the build artifacts from the previous stage
COPY --from=build-env /app/out .

# Expose the default port to run
EXPOSE 8080

# Start the application
ENTRYPOINT [ "dotnet", "Transport-Management-Systems-Portal-Order-Service-REST-API.dll" ]