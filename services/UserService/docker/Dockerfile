# ---------- 0. build-time аргументы ----------
ARG DOTNET_VERSION=9.0
ARG PROJECT_NAME=UserService
ARG CONFIGURATION=Release

# ---------- 1. Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# объявляем заново, чтобы пользоваться в stage
ARG PROJECT_NAME
ARG CONFIGURATION

# 1.1 Копируем .sln (чтобы restore по solution, если захотите)
COPY services/${PROJECT_NAME}/src/ ./services/${PROJECT_NAME}/src/
COPY shared/**/*.csproj          ./shared/
RUN dotnet restore \
    "services/${PROJECT_NAME}/src/${PROJECT_NAME}.Api/${PROJECT_NAME}.Api.csproj"

# 1.3 Копируем остальной код
COPY shared/         ./shared/
COPY services/${PROJECT_NAME}/src/  ./services/${PROJECT_NAME}/src/
RUN dotnet publish \
    "services/${PROJECT_NAME}/src/${PROJECT_NAME}.Api/${PROJECT_NAME}.Api.csproj" \
    -c ${CONFIGURATION} -o /app/publish

# ---------- 2. Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "UserService.Api.dll"]