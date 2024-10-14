# Plugin installation

1. Download latest release file
2. Desktop ->
3. Development Tools ->
4. Instalation Wizard -> 
5. Upload Package -> 
6. Choose file ->
7. OverWrite existing files - check ->
8. Choose package -> 
9. Next -> 
10. Install ->
11. (if it no first installation) overwrite - yes to all ->
12. (if it no first installation) for items overWrite - apply to all -> 
13. Restart the Sitecore client - check
14. Close

# Установка sitecore
Ссылка на туториал по установке https://dev.to/esdanielgomez/install-sitecore-10-step-by-step-using-sia-2f2j 

Локальная базу в mssql нужно сделать с аутентификацией по логину и паролю, юзер sa 

Для развертывания SmartcatPlugin нужно сбидить и скопировать файлы в инстанс сайткора:

1. SmartcatPlugin.dll и SmartcatPlugin.pdb скопировать в папку C:\inetpub\wwwroot\sc10sc.dev.local\bin

2. Папку ..\SmartcatPlugin\SmartcatPlugin\src\platform\App_Config\Include\Routes скопировать в C:\inetpub\wwwroot\sc10sc.dev.local\App_Config\Include

3. Файл ..\SmartcatPlugin\SmartcatPlugin\src\platform\App_Config\Include\Commands.config скопировать в C:\inetpub\wwwroot\sc10sc.dev.local\App_Config\Include

4. Перезагрузить iis для sc10sc.dev.local

После внесения изменений в код, повторно сбилдить и скопировать SmartcatPlugin.dll и SmartcatPlugin.pdb и перезагрузить iis для sc10sc.dev.local
