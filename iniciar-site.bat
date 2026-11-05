@echo off
title Farma e Cia
setlocal

set "PASTA_PROJETO=%~dp0FarmaECia_Interface_Basica\FarmaECia_Interface_Basica\FarmaECia"

if not exist "%PASTA_PROJETO%\FarmaECia.csproj" (
    echo Nao foi possivel encontrar o projeto FarmaECia.
    echo Verifique se a pasta do projeto esta no lugar esperado.
    pause
    exit /b 1
)

cd /d "%PASTA_PROJETO%"

echo Iniciando a Farma^&Cia...
echo O site abrira em http://localhost:5186
echo Para encerrar, pressione Ctrl+C nesta janela.
echo.

dotnet run --launch-profile "FarmaECia - http"

pause
