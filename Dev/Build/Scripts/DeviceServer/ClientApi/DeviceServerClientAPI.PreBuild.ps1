Param ($context)

### 01. Чистим целевой каталог доставки

# DeviceServerClientAPI

$context.Logger.Info("DeviceServerClientAPI PreBuild Action executing")

$cleanPath   = Join-Path -Path $context.RootFolder -ChildPath "\Dev\Delivery\Atdi.Sdrn\Device\ClientApi\BinDebug"
$cleanFilter = "atdi.*.*"
Get-ChildItem -Path $cleanPath -File -Filter $cleanFilter | Remove-Item -Recurse -Force -Confirm:$false


$cleanPath   = Join-Path -Path $context.RootFolder -ChildPath "\Dev\Delivery\Atdi.Sdrn\Device\ClientApi\BinRelease"
$cleanFilter = "atdi.*.*"
Get-ChildItem -Path $cleanPath -File -Filter $cleanFilter | Remove-Item -Force -Recurse -Confirm:$false



$context.Logger.Info("DeviceServerClientAPI PreBuild Action was executed")
$context.Logger.Info("");