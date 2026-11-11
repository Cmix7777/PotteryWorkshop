@echo off
chcp 65001 >nul
cd /d "%~dp0"
taskkill /im PotteryWorkshop.exe /f >NUL 2>&1
echo Сайт запускается, адрес http://localhost:5080
echo Не закрывайте это окно, пока работаете с сайтом.
dotnet run --launch-profile http > run-log.txt 2>&1
