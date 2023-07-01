FROM mcr.microsoft.com/dotnet/aspnet:6.0-jammy-arm64v8 AS base
LABEL maintainer="Tacio de Souza Campos"
EXPOSE 80
EXPOSE 443
WORKDIR /app


FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ENV ASPNETCORE_URLS=http://*:$PORT
WORKDIR /src
COPY src/*.csproj .
RUN dotnet restore

COPY src .
RUN dotnet build -c Release --no-restore -o /app

# WORKDIR /tests
# COPY tests/SACA.Tests.Integration .
# RUN dotnet test
# COPY tests/SACA.UnitTests .
# RUN dotnet test


FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SACA.dll"]