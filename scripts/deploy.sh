#!/bin/bash
cd /var/www/saca/src
dotnet restore
dotnet build --configuration Release --no-restore
dotnet publish --configuration Release --no-build
dotnet-ef database update
