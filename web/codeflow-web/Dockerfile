# ---------- 1. Stage: build ----------
FROM node:20 AS build
WORKDIR /app

# Копируем package.json и package-lock.json
COPY package*.json ./

# Устанавливаем зависимости
RUN npm install

# Копируем исходники
COPY . .

# Сборка production билда
RUN npm run build

# ---------- 2. Stage: serve ----------
FROM nginx:alpine

# Копируем свой конфиг nginx
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Копируем билд React в папку nginx
COPY --from=build /app/build /usr/share/nginx/html

# Порт
EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]