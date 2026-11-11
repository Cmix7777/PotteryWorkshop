#!/bin/sh
# Запуск сайта на Layero
if [ -z "$(find / -name 'libicuuc.so.*' 2>/dev/null | head -n 1)" ]; then
  export DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
fi
export DOTNET_SYSTEM_GLOBALIZATION_PREDEFINED_CULTURES_ONLY=false
export ASPNETCORE_URLS="http://0.0.0.0:${PORT:-3000}"
export ConnectionStrings__Default="Data Source=/tmp/pottery.db"
exec ./out/PotteryWorkshop
