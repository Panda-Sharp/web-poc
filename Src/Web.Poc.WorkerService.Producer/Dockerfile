# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80
EXPOSE 443


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Web.Poc.WorkerService.Producer/Web.Poc.WorkerService.Producer.csproj", "Web.Poc.WorkerService.Producer/"]
COPY ["Web.Poc.Application.Contracts/Web.Poc.Application.Contracts.csproj", "Web.Poc.Application.Contracts/"]
COPY ["Web.Poc.Domain/Web.Poc.Domain.csproj", "Web.Poc.Domain/"]
COPY ["Web.Poc.Infrastructure/Web.Poc.Infrastructure.csproj", "Web.Poc.Infrastructure/"]
COPY ["Web.Poc.Application/Web.Poc.Application.csproj", "Web.Poc.Application/"]
RUN dotnet restore "./Web.Poc.WorkerService.Producer/Web.Poc.WorkerService.Producer.csproj"
COPY . .
WORKDIR "/src/Web.Poc.WorkerService.Producer"
RUN dotnet build "./Web.Poc.WorkerService.Producer.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Web.Poc.WorkerService.Producer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Web.Poc.WorkerService.Producer.dll"]