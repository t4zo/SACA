FROM mcr.microsoft.com/dotnet/core/sdk:3.1-bionic AS build
WORKDIR /var/www/app
ENV ASPNETCORE_URLS=http://*:$PORT
COPY "SACA.csproj" .
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /var/www/app/build

FROM build AS publish
RUN dotnet publish -c Release -o /var/www/app/publish

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-bionic AS runtime
LABEL maintainer="Tacio de Souza Campos"
WORKDIR /var/www/app
COPY --from=publish /var/www/app/publish .
ENTRYPOINT ["dotnet", "SACA.dll"]