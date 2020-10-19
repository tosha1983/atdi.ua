cls


### Этап 01 - Действия перед сборкой - это может быть очистка катлоаг артифактов от предыдущих компонентов
$result = $context.Execute("DeviceServerCore.PreBuild.ps1")

### Этап 02 - Сборка проекта
$result = $context.Execute("DeviceServerCore.Build.ps1")

### Этап 03 - Копирование методаных
$result = $context.Execute("DeviceServerCore.PosBuild.ps1")
