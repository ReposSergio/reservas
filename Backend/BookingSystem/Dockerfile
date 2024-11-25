# Usa la imagen base de .NET SDK para compilar y publicar
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copia los archivos del proyecto y restaura las dependencias
COPY BookingSystem.csproj ./
RUN dotnet restore

# Copia el resto del código y compila
COPY . ./
RUN dotnet publish -c Release -o /publish

# Usa una imagen runtime para reducir el tamaño
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /publish .

# Exponemos el puerto de la API
EXPOSE 5000


# Comando de inicio de la aplicación
ENTRYPOINT ["dotnet", "BookingSystem.dll"]
