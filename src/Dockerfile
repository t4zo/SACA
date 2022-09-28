FROM mcr.microsoft.com/dotnet/aspnet:6.0-jammy-arm64v8 AS base
LABEL maintainer="Tacio de Souza Campos"
EXPOSE 80
EXPOSE 443
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ENV ASPNETCORE_URLS=http://*:$PORT
WORKDIR /src
COPY *.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet build --no-restore -o /app

FROM build AS publish
RUN dotnet publish -o /app

FROM mcr.microsoft.com/dotnet/aspnet:6.0-jammy-arm64v8
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SACA.dll"]