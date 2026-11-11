#!/bin/sh
# Сборка сайта на Layero: ставим .NET SDK во временную папку и публикуем
# готовое приложение (self-contained) в папку out
set -e
RID=linux-x64
[ -f /etc/alpine-release ] && RID=linux-musl-x64
echo "Платформа: $RID"

# Системные пакеты: сертификаты для NuGet, ICU для русских дат, SQLite
if command -v apt-get >/dev/null 2>&1; then
  (apt-get update && apt-get install -y --no-install-recommends ca-certificates libicu72 libsqlite3-0) \
    || (apt-get install -y --no-install-recommends ca-certificates 'libicu[0-9]*' libsqlite3-0) \
    || echo "apt-get не сработал, продолжаем"
fi
# Если системных сертификатов нет, берём их из Node.js
if [ ! -s /etc/ssl/certs/ca-certificates.crt ]; then
  node -e "console.log(require('tls').rootCertificates.join('\n'))" > /tmp/node-ca.pem
  export SSL_CERT_FILE=/tmp/node-ca.pem
  echo "Используем сертификаты из Node.js"
fi

export DOTNET_CLI_TELEMETRY_OPTOUT=1 DOTNET_NOLOGO=1 DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
node -e "fetch('https://aka.ms/dotnet/8.0/dotnet-sdk-'+process.argv[1]+'.tar.gz').then(r=>{if(!r.ok)throw new Error('HTTP '+r.status);return r.arrayBuffer()}).then(b=>require('fs').writeFileSync('/tmp/sdk.tgz',Buffer.from(b)))" "$RID"
mkdir -p /tmp/dotnet
tar -xzf /tmp/sdk.tgz -C /tmp/dotnet
rm -f /tmp/sdk.tgz
# Встроенная библиотека SQLite, чтобы не зависеть от системной
/tmp/dotnet/dotnet add PotteryWorkshop.csproj package SQLitePCLRaw.lib.e_sqlite3 --version 2.1.6
/tmp/dotnet/dotnet publish PotteryWorkshop.csproj -c Release -r "$RID" --self-contained true -o out
cp out/libe_sqlite3.so out/libsqlite3.so
rm -rf /tmp/dotnet bin obj
echo "Сборка готова"
