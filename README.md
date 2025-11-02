# О проекте 
Проект находится в разработке, моя кастомная реализация хелперов, позволяющая упростить собственную жизнь, используется для сбора информации с git, собирает коммиты по соглашению о коммитах, и превращает их в Changelog, для подготовки результирующих документов, о проделанной работе.
На что способна данная версия: очищать директории, формировать валидный json с git log и формировать на основе них полноценный pdf Changelog.

# Dependencies
Microsoft.Extensions.DependencyInjection.Abstractions (DI container)
Microsoft.Extensions.Options.ConfigurationExtensions (configuration binding)
Serilog (logging)
QuestPDF (PDF generation engine)
Newtonsoft.Json (JSON serialization)

# Как запустить и сформировать рабочий EXE
Пример на Windows CMD / PowerShell.

REM 1. Клонировать репозиторий
git clone GitChronicle

REM 2. Перейти в директорию проекта
cd <ваш путь>\GitChronicle

REM 3. Собрать проект в режиме Release
dotnet publish -c Release -o ./publish

REM 4. Перейти в директорию с собранным EXE
cd publish

REM 5. Запустить консольное приложение с аргументами
Changeloger.exe ^
    --platform PlatformName ^
    --user user ^
    --token token ^
    --repolist "<ваш путь>\Repositories\repositories.txt" ^
    --build "<ваш путь>\tmp_changeloger\Build" ^
    --since 2025-08-01 ^
    --update
	
# Список аттрибутов
--platform - имя
--user - имя пользователя git
--token - token для доступа git
--repolist -- на данный момент поддерживает только список репозитория с файловой системы, например блокнот документ со списком проектов. Как оформлять документ репозиториев (список проектов один проект на строчку с переходом на новую строчку, без запятых и других разделителей https://gitlab.ru/project1.git)
--build - корневой каталог временных и конечных артефактов
--since - с какого момента собирать логи
--update - если выставлен данный флаг то будет пытаться обновить существующие проекты в директории <--build>\Projects, не обязательный аттрибут. без флага update приложение будет пытаться сделать clean, init, log, changelog. Если выставлен update то clean(только директорию с  временными результатами json), update, log, changelog

# Как передать параметры Debug mode
Создать в проекте консольного приложения Properties/launchSettings.json
Добавить следующий профиль
"profiles": {
  "Changeloger": {
    "commandName": "Project",
    "commandLineArgs": "--platform xxx --user user --token token --repolist \"ваш путь\\Repositories\\repositories.txt\" --build \"ваш путь\\Build\" --since 2025-08-01 --update"
  }
}