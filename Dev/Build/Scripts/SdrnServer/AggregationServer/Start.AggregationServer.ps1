cls


### Этап 01 - Действия перед сборкой - это может быть очистка катлоаг артифактов от предыдущих компонентов
$result = $context.Execute("AggregationServer.PreBuild.ps1")

### Этап 02 - Сборка проекта
$result = $context.Execute("AggregationServer.Build.ps1")

