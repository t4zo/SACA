#!/bin/bash
sudo systemctl stop saca
cd /var/www/saca/src
dotnet restore
dotnet build --configuration Release --no-restore
dotnet publish --configuration Release --no-build
dotnet-ef database update
sudo systemctl start saca
