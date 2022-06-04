# FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
# WORKDIR /usr/app
# ENV ASPNETCORE_URLS=http://*:$PORT

# # Copy csproj and restore as distinct layers
# COPY src/*.csproj ./
# RUN dotnet restore

# # Copy files and build
# COPY . .
# RUN dotnet build -c Release

# # Publish
# FROM build AS publish
# RUN dotnet publish --no-restore --no-build -c Release -o out

# # Build runtime image
# FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
# LABEL maintainer="Tacio de Souza Campos"
# EXPOSE 80
# EXPOSE 443
# WORKDIR /usr/app
# COPY --from=publish /usr/app/out .
# ENTRYPOINT ["dotnet", "SACA.dll"]

#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY *.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release --no-restore --no-build -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SACA.dll"]