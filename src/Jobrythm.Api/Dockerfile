FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Jobrythm.Api/Jobrythm.Api.csproj", "src/Jobrythm.Api/"]
COPY ["src/Jobrythm.Application/Jobrythm.Application.csproj", "src/Jobrythm.Application/"]
COPY ["src/Jobrythm.Domain/Jobrythm.Domain.csproj", "src/Jobrythm.Domain/"]
COPY ["src/Jobrythm.Infrastructure/Jobrythm.Infrastructure.csproj", "src/Jobrythm.Infrastructure/"]
RUN dotnet restore "src/Jobrythm.Api/Jobrythm.Api.csproj"
COPY . .
WORKDIR "/src/src/Jobrythm.Api"
RUN dotnet build "Jobrythm.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Jobrythm.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Jobrythm.Api.dll"]
