# ETAPA 1: Build (Construcción)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiamos primero el csproj y restauramos dependencias (para aprovechar la caché de Docker)
COPY ["KindergartenAPI.csproj", "./"]
RUN dotnet restore "KindergartenAPI.csproj"

# Copiamos el resto del código y compilamos
COPY . .
RUN dotnet publish "KindergartenAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ETAPA 2: Runtime (Ejecución)
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

# Exponemos el puerto estándar de .NET 9 (8080)
EXPOSE 8080

# Comando de arranque
ENTRYPOINT ["dotnet", "KindergartenAPI.dll"]