#!/bin/bash
cd /var/www/saca/src
dotnet restore
dotnet build --configuration Release --no-restore
dotnet publish --configuration Release --no-restore --no-build
