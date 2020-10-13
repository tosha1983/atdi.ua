cls

### Этап 01 - Сборка DeviceServerClientAPI
$result = $context.Execute("Start.DeviceServerClientAPI.ps1")

### Этап 02 - Сборка DeviceServerAdapter
$result = $context.Execute("Start.DeviceServerAdapter.ps1")

### Этап 03 - Сборка DeviceServerCore
$result = $context.Execute("Start.DeviceServerCore.ps1")

### Этап 04 - Сборка DeviceServerProcessing
$result = $context.Execute("Start.DeviceServerProcessing.ps1")
