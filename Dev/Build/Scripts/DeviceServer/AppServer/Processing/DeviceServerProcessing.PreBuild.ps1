Param ($context)

### 01. Чистим целевой каталог доставки

# DeviceServerProcessing

$context.Logger.Info("DeviceServerProcessing PreBuild Action executing")

$cleanPath   = Join-Path -Path $context.RootFolder -ChildPath "\Dev\Delivery\Atdi.Sdrn\Device\AppServer\Processing\BinDebug"
$cleanFilter = "atdi.*.*"
Get-ChildItem -Path $cleanPath -File -Filter $cleanFilter | Remove-Item -Recurse -Force -Confirm:$false


$cleanPath   = Join-Path -Path $context.RootFolder -ChildPath "\Dev\Delivery\Atdi.Sdrn\Device\AppServer\Processing\BinRelease"
$cleanFilter = "atdi.*.*"
Get-ChildItem -Path $cleanPath -File -Filter $cleanFilter | Remove-Item -Force -Recurse -Confirm:$false



$context.Logger.Info("DeviceServerProcessing PreBuild Action was executed")
$context.Logger.Info("");