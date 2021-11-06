# FROM mcr.microsoft.com/dotnet/core/sdk:3.1-bionic AS build
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /var/www/app
ENV ASPNETCORE_URLS=http://*:$PORT

# Copy csproj and restore dependencies
COPY *.csproj .
RUN dotnet restore

# Copy and build project files
COPY . .
RUN dotnet build -c Release -o /var/www/app/build

#  Publish project
FROM build AS publish
RUN dotnet publish -c Release -o /var/www/app/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
LABEL maintainer="Tacio de Souza Campos"
EXPOSE 80
EXPOSE 443
WORKDIR /var/www/app
COPY --from=publish /var/www/app/publish .
ENTRYPOINT ["dotnet", "SACA.dll"]