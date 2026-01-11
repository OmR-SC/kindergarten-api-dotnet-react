# ETAPA 1: Build con Node.js
FROM node:18-alpine AS build
WORKDIR /app

# Instalar dependencias
COPY package.json package-lock.json* ./
RUN npm install

ARG VITE_API_URL

ENV VITE_API_URL=$VITE_API_URL


# Copiar código y construir
COPY . .
RUN npm run build

# ETAPA 2: Servir con Nginx
FROM nginx:alpine
# Copiamos los archivos construidos (dist) a la carpeta pública de Nginx
COPY --from=build /app/dist /usr/share/nginx/html

COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]