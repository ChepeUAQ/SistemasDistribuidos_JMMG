FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

ENV ASPNETCORE_URLS=http://+:80

USER root 
RUN mkdir /app/tmp && echo "Morales Gomez Jose Manuel" > /app/tmp/315316.txt
USER app 

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["CrearArchivo.csproj", "./"]
RUN dotnet restore "CrearArchivo.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "CrearArchivo.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "CrearArchivo.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .