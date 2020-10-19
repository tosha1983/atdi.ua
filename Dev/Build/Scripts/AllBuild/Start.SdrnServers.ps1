cls


### Этап 01 - Сборка SdrnServer
$result = $context.Execute("Start.SdrnServer.ps1")

### Этап 02 - Сборка AggregationServer
$result = $context.Execute("Start.AggregationServer.ps1")

### Этап 03 - Сборка MasterServer
$result = $context.Execute("Start.MasterServer.ps1")

### Этап 04 - Сборка WcfService
$result = $context.Execute("Start.WcfService.ps1")
