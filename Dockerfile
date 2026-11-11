# Сборка
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY PotteryWorkshop.csproj .
RUN dotnet restore PotteryWorkshop.csproj
COPY . .
RUN dotnet publish PotteryWorkshop.csproj -c Release -o /app

# Запуск
FROM mcr.microsoft.com/dotnet/aspnet:8.0
# Системная библиотека SQLite для базы данных
RUN apt-get update && apt-get install -y --no-install-recommends libsqlite3-0 && rm -rf /var/lib/apt/lists/* \
    && ln -s "$(find /usr/lib -name 'libsqlite3.so.0' | head -n 1)" /usr/lib/libsqlite3.so
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://+:8080
# База лежит в /data, чтобы на хостинге её можно было сохранять между перезапусками
RUN mkdir -p /data
ENV ConnectionStrings__Default="Data Source=/data/pottery.db"
EXPOSE 8080
ENTRYPOINT ["dotnet", "PotteryWorkshop.dll"]
