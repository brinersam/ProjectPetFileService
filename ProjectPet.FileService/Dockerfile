# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 6060
EXPOSE 6061


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release

ARG NUGET_USERNAME
ARG NUGET_PASSWORD

WORKDIR /src

RUN dotnet nuget add source https://nuget.pkg.github.com/brinersam/index.json \
-n github \
--username $NUGET_USERNAME \
--password $NUGET_PASSWORD \
--store-password-in-clear-text

COPY ["Directory.Build.props", "."]
COPY ["ProjectPet.FileService/ProjectPet.FileService.csproj", "ProjectPet.FileService/"]
RUN dotnet restore "./ProjectPet.FileService/ProjectPet.FileService.csproj"
COPY . .
WORKDIR "/src/ProjectPet.FileService"
RUN dotnet build "./ProjectPet.FileService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ProjectPet.FileService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProjectPet.FileService.dll"]