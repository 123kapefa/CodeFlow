# ---------- 0. build-time аргументы ----------
ARG DOTNET_VERSION=9.0-preview
ARG PROJECT_NAME=AuthService
ARG CONFIGURATION=Release

# ---------- 1. Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
WORKDIR /src

# объявляем заново, чтобы пользоваться в stage
ARG PROJECT_NAME
ARG CONFIGURATION

# 1.1 Копируем .sln (чтобы restore по solution, если захотите)
COPY services/${PROJECT_NAME}/src/ ./services/${PROJECT_NAME}/src/

# 1.2 только *.csproj (кэш слоя restore)
COPY services/${PROJECT_NAME}/src/**/*.csproj ./services/${PROJECT_NAME}/src/
COPY shared/BuildingBlocks/**/*.csproj          ./shared/BuildingBlocks/
RUN dotnet restore \
    "services/${PROJECT_NAME}/src/${PROJECT_NAME}.Api/${PROJECT_NAME}.Api.csproj"

# 1.3 Копируем остальной код
COPY shared/BuildingBlocks/         ./shared/BuildingBlocks/
COPY services/${PROJECT_NAME}/src/  ./services/${PROJECT_NAME}/src/
RUN dotnet publish \
    "services/${PROJECT_NAME}/src/${PROJECT_NAME}.Api/${PROJECT_NAME}.Api.csproj" \
    -c ${CONFIGURATION} -o /app/publish

# ---------- 2. Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "AuthService.Api.dll"]





# # 1. Сборка
# FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
# WORKDIR /src

# COPY ["AuthService.Api/AuthService.Api.csproj", "AuthService.Api/"]
# COPY ["AuthService.Application/AuthService.Application.csproj", "AuthService.Application/"]
# COPY ["AuthService.Domain/AuthService.Domain.csproj", "AuthService.Domain/"]
# COPY ["AuthService.Infrastructure/AuthService.Infrastructure.csproj", "AuthService.Infrastructure/"]

# RUN dotnet restore "AuthService.Api/AuthService.Api.csproj"

# COPY . .

# RUN dotnet publish "AuthService.Api/AuthService.Api.csproj"  \
#     -c Release -o /app/publish /p:UseAppHost=false

# # 2. Запуск
# FROM mcr.microsoft.com/dotnet/aspnet:9.0-preview AS runtime
# WORKDIR /app

# COPY --from=build /app/publish .

# ENV ASPNETCORE_URLS=http://+:5000
# EXPOSE 5000

# ENTRYPOINT ["dotnet", "AuthService.Api.dll"]