# Paso 1: Usar el SDK de .NET 10 para compilar el proyecto
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
WORKDIR /app

# Copiar los archivos del proyecto y restaurar las dependencias
COPY *.sln ./
COPY *.csproj ./
RUN dotnet restore

# Copiar todo lo demás y compilar en modo Release
COPY . ./
RUN dotnet publish -c Release -o out

# Paso 2: Construir la imagen final ligera para ejecución con el runtime de .NET 10
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build-env /app/out .

# Configurar el puerto.
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "AthletaNet.dll"]
