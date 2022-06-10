FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
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
RUN dotnet build -o /app

FROM build AS publish
RUN dotnet publish --no-restore --no-build -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SACA.dll"]