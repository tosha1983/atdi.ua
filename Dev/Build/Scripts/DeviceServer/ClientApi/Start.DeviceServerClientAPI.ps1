cls


### Этап 01 - Действия перед сборкой - это может быть очистка катлоаг артифактов от предыдущих компонентов
$result = $context.Execute("DeviceServerClientAPI.PreBuild.ps1")

### Этап 02 - Сборка проекта
$result = $context.Execute("DeviceServerClientAPI.Build.ps1")

### Этап 03 - Копирование методаных
$result = $context.Execute("DeviceServerClientAPI.PosBuild.ps1")
