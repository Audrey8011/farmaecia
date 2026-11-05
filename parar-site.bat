@echo off
title Parar Farma e Cia
setlocal

echo Encerrando a Farma^&Cia em execução local...
for /f "tokens=5" %%p in ('netstat -ano ^| findstr /I ":5186" ^| findstr /I "LISTENING"') do taskkill /PID %%p /F >nul 2>&1
for /f "tokens=5" %%p in ('netstat -ano ^| findstr /I ":7186" ^| findstr /I "LISTENING"') do taskkill /PID %%p /F >nul 2>&1

echo Pronto. Agora você pode executar pelo Visual Studio (F5) ou pelo iniciar-site.bat.
pause
