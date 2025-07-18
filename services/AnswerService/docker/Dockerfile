# ---------- 0. build-time аргументы ----------
ARG DOTNET_VERSION=9.0-preview
ARG PROJECT_NAME=AuthService
ARG CONFIGURATION=Release

# ---------- 1. Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# объявляем заново, чтобы пользоваться в stage
ARG PROJECT_NAME
ARG CONFIGURATION

# 1.1 Копируем shared целиком (до restore!), чтобы dotnet увидел Contracts.csproj
COPY shared/ ./shared/

# 1.2 Копируем csproj нашего сервиса (для кэша слоя restore)
COPY services/AnswerService/src/AnswerService.Api/AnswerService.Api.csproj \
     services/AnswerService/src/AnswerService.Api/
COPY services/AnswerService/src/AnswerService.Application/AnswerService.Application.csproj \
     services/AnswerService/src/AnswerService.Application/
COPY services/AnswerService/src/AnswerService.Domain/AnswerService.Domain.csproj \
     services/AnswerService/src/AnswerService.Domain/
COPY services/AnswerService/src/AnswerService.Infrastructure/AnswerService.Infrastructure.csproj \
     services/AnswerService/src/AnswerService.Infrastructure/

# 1.3 Восстанавливаем зависимости
RUN dotnet restore \
    "services/${PROJECT_NAME}/src/${PROJECT_NAME}.Api/${PROJECT_NAME}.Api.csproj"

# 1.4 Копируем уже весь код сервиса
COPY services/${PROJECT_NAME}/src/ ./services/${PROJECT_NAME}/src/

# 1.5 Публикуем в /app/publish
RUN dotnet publish \
    "services/${PROJECT_NAME}/src/${PROJECT_NAME}.Api/${PROJECT_NAME}.Api.csproj" \
    -c ${CONFIGURATION} -o /app/publish

# ---------- 2. Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "AnswerService.Api.dll"]
