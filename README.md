# CodeFlow  

🚀 **CodeFlow** — это проект в формате системы вопросов и ответов, построенный на **микросервисной архитектуре** с использованием **.NET 9**, **React** и **Docker**.  

---

## 📌 Возможности  

- 🔐 **Аутентификация и авторизация** (JWT + OAuth: Google, GitHub)  
- ❓ **Работа с вопросами** (создание, редактирование, удаление, просмотр)  
- 💬 **Ответы и комментарии** (возможность выбора «лучшего ответа»)  
- 🏷 **Теги** (автодополнение, ограничение до 5 тегов)  
- 👍 **Голосование** (upvote/downvote, репутация пользователей)  
- 👤 **Профиль пользователя** (статистика, репутация, аватарки через S3)  
- 🔎 **Поиск и сортировка** (по тегам, популярности, дате создания)  
- 📢 **Система рекомендаций** (персонализированные вопросы)  

---

## 🏗 Архитектура  

Проект реализован как набор микросервисов, каждый со своей БД:  

- **AuthService** — регистрация, логин, JWT, OAuth  
- **UserService** — хранение профилей пользователей, статистика  
- **QuestionService** — управление вопросами  
- **AnswerService** — работа с ответами  
- **CommentService** — система комментариев  
- **TagService** — работа с тегами  
- **ReputationService** — работа с репутацией  
- **VoteService** — система голосов  
- **ApiGateway** — единая точка входа (обработка запросов, агрегация данных)  
- **Frontend (React + Nginx)** — клиентская часть  

Инфраструктурные компоненты:  

- **PostgreSQL** — основная СУБД  
- **RabbitMQ** — брокер сообщений между сервисами  
- **Redis** — кеш и хранение сессий  
- **Nginx** — reverse proxy + отдача фронтенда  

---

## 🛠 Технологии  

- **Backend**: C#, ASP.NET Core 9, Entity Framework Core, MassTransit  
- **Frontend**: React, Bootstrap  
- **Инфраструктура**: Docker, Docker Compose, Yandex Cloud (VM, S3)  
- **Хранилище файлов**: Yandex Object Storage (S3-совместимое)  

---

## ⚙️ Запуск проекта  

### 1. Клонирование репозитория  

```bash
git clone https://github.com/123kapefa/CodeFlow.git
cd CodeFlow
git checkout features/remote/remote-1-setting
```
## Создание .env файлов

Для каждого сервиса и фронтенда должны быть .env файлы (см. примеры в docs/.env.example).

Пример основных переменных:
```dotenv
ASPNETCORE_ENVIRONMENT=Development
POSTGRES_USER=codeflow
POSTGRES_PASSWORD=codeflow_pwd
RABBITMQ__HOST=rabbitmq
REDIS__CONNECTION=redis:6379
JWT__KEY=super_secret_key
```

## Запуск через Docker Compose
```bash
docker compose build
docker compose up -d
```

После запуска сервисы будут доступны:

🌐 Веб-клиент: http://localhost:3000

📘 Swagger ApiGateway: http://localhost:5000/swagger

🐇 RabbitMQ UI: http://localhost:15672
 (логин/пароль: guest / guest)

## 🧪 Тестирование

Функциональное — CRUD для вопросов, ответов, тегов

Интеграционное — взаимодействие сервисов через RabbitMQ

Нагрузочное — проверка производительности при росте числа пользователей

## 📈 Планы развития

📩 Уведомления (NotificationService)

🔍 Улучшенный поиск (SearchService)

🏅 Бейджи и достижения

🤖 ML-рекомендации

## 👨‍💻 Наша команда

Александр Трунин

GitHub: [Hissul](https://github.com/Hissul)

Артур Соколов

GitHub: [123kapefa](https://github.com/123kapefa)
