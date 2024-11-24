# Usa una imagen base de Node.js para la compilación
FROM node:18 AS build
WORKDIR /app

# Copia los archivos del proyecto
COPY package.json package-lock.json ./
RUN npm install

# Copia el resto del código y construye la aplicación
COPY . ./
RUN npm run build

# Usa una imagen ligera de Node.js para el runtime
FROM node:18-alpine
WORKDIR /app
COPY --from=build /app ./

# Exponemos el puerto de Next.js
EXPOSE 3000

# Comando de inicio de la aplicación
CMD ["npm", "start"]
