Ссылка на туториал по установке https://dev.to/esdanielgomez/install-sitecore-10-step-by-step-using-sia-2f2j 

Локальная базу в mssql нужно сделать с аутентификацией по логину и паролю, юзер sa 

Для развертывания SmartcatPlugin нужно сбидить и перетащить файлы в инстанс сайткора:

SmartcatPlugin.dll и SmartcatPlugin.pdb в папку C:\inetpub\wwwroot\sc10sc.dev.local\bin

Папку ..\SmartcatPlugin\SmartcatPlugin\src\platform\App_Config\Include\Routes скопировать в C:\inetpub\wwwroot\sc10sc.dev.local\App_Config\Include
