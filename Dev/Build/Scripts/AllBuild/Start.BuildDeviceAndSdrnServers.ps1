cls

### 00. Путь к размещению сборки
$sourceRootFolder = "D:\Build"
$reposPath = "https://kovpak.visualstudio.com/atdi.ua/_git/atdi.ua"

### Создание контекста сборки
$scriptsPath   = $PSScriptRoot
$createContext = $scriptsPath   + "\Builder.CreateContext.ps1"
$context = . $createContext -sourceRootFolder $sourceRootFolder -scriptsPath $scriptsPath -reposPath $reposPath

### Выводим в лог начало процесса сборки
$context.Logger.Info("Start test building")
$logger.Info("");


### Этап 01 - Очистка каталаога сборки
$result = $context.Execute("Builder.ClearRootFolder.ps1")

### Этап 02 - Клонирование репозитария
$result = $context.Execute("Builder.CloneRepos.ps1")

### Этап 03 - Востанавливаем NuGet-пакеты с репозитария
$result = $context.Execute("Builder.LoadNugetPackages.ps1")

### Этап 04 - Сборка DeviceServer
$result = $context.Execute("Start.DeviceServer.ps1")

### Этап 05 - Сборка SdrnServers
$result = $context.Execute("Start.SdrnServers.ps1")


