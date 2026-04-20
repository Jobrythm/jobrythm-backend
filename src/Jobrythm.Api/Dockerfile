# Use the official .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
WORKDIR /app

# Copy solution and project files first to leverage Docker layer caching
COPY Jobrythm.slnx ./
COPY src/Jobrythm.Domain/Jobrythm.Domain.csproj src/Jobrythm.Domain/
COPY src/Jobrythm.Application/Jobrythm.Application.csproj src/Jobrythm.Application/
COPY src/Jobrythm.Api/Jobrythm.Api.csproj src/Jobrythm.Api/

# Restore dependencies
RUN dotnet restore

# Copy the rest of the source code
COPY src/ ./src/

# Build and publish the API project
WORKDIR /app/src/Jobrythm.Api
RUN dotnet publish -c Release -o /app/publish

# Use the official .NET ASP.NET runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build-env /app/publish .

# Expose the port the app runs on
EXPOSE 8080

# Set environment variables if needed
ENV ASPNETCORE_URLS=http://+:8080
ENV AppConfig__BaseUrl=https://api.jobrythm.com
ENV AppConfig__ClientUrl=https://app.jobrythm.com

ENTRYPOINT ["dotnet", "Jobrythm.Api.dll"]
