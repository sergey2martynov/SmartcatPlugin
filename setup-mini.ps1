#На основе инструкции из репозитория https://github.com/sergey2martynov/SmartcatPlugin
#Установить MSSQL
#Скачать и установить sitecore (https://scdp.blob.core.windows.net/downloads/Sitecore%20Experience%20Platform/104/Sitecore%20Experience%20Platform%20104/Sitecore%2010.4.0%20rev.%20010422%20(Setup%20XP0%20Developer%20Workstation%20rev.%201.6.0-r4).zip)
#Задать:
#	Путь к репозиторию SmartcatPlugin
$rep = "d:\Projects\Smartcat\SmartcatPlugin\"
#	Префикс, указанный при установке sitecore (можно взять из названия сайта <prefix>sc.dev.local)
$prefix = "score1"
#Запустить скрипт и перезапустить IIS
#Проверить методом 
#HTTP POST https://test4sc.dev.local/smartcat/directory-list Body:{"batchkey": null,"parentDirectoryId": {"externalId" : "root","externalType" : "folder"}}

# Install NUGET packages
# https://sitecore.stackexchange.com/questions/26019/asp-net-core-renderinghost-in-experience-editor
# Add SiteCore nuget plugins



# Константы
$source_dll_path = "$rep\SmartcatPlugin\src\platform\bin"
$source_include_path = "$rep\SmartcatPlugin\src\platform\App_Config\Include"

# Задаем целевые пути
$target_base = "C:\inetpub\wwwroot"
$sc = $prefix + "sc.dev.local"
$target_bin_path = "$target_base\$sc\bin"
$target_include_path = "$target_base\$sc\App_Config\Include"

# Включаем остановку при ошибке
$ErrorActionPreference = "Stop"


    Copy-Item -Path "$source_dll_path\SmartcatPlugin.dll" -Destination "$target_bin_path" -Force -ErrorAction Stop
    Write-Host "SmartcatPlugin.dll copied"

    Copy-Item -Path "$source_dll_path\SmartcatPlugin.pdb" -Destination "$target_bin_path" -Force -ErrorAction Stop
    Write-Host "SmartcatPlugin.pdb copied"
