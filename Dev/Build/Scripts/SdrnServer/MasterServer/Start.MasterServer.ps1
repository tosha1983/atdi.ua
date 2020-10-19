cls


### Этап 01 - Действия перед сборкой - это может быть очистка катлоаг артифактов от предыдущих компонентов
$result = $context.Execute("MasterServer.PreBuild.ps1")

### Этап 02 - Сборка проекта
$result = $context.Execute("MasterServer.Build.ps1")

