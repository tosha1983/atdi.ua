cls

### Выводим в лог начало процесса сборки
$context.Logger.Info("Start test building")
$logger.Info("");


### Этап 01 - Действия перед сборкой - это может быть очистка катлоаг артифактов от предыдущих компонентов
$result = $context.Execute("DeviceServerAdapter.PreBuild.ps1")

### Этап 02 - Сборка проекта
$result = $context.Execute("DeviceServerAdapter.Build.ps1")


