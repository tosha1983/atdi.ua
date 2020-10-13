Param ($context)

### 01. Чистим целевой каталог доставки

# DeviceServerCore

$context.Logger.Info("DeviceServerProcessing PreBuild Action executing")

$cleanPath   = Join-Path -Path $context.RootFolder -ChildPath "\Dev\Delivery\Atdi.Sdrn\Device\AppServer\Core\BinDebug"
$cleanFilter = "atdi.*.*"
Get-ChildItem -Path $cleanPath -File -Filter $cleanFilter | Remove-Item -Recurse -Force -Confirm:$false

$cleanPath   = Join-Path -Path $context.RootFolder -ChildPath "\Dev\Delivery\Atdi.Sdrn\Device\AppServer\Core\BinDebug\Metadata"
$cleanFilter = "*.*"
Get-ChildItem -Path $cleanPath -Filter $cleanFilter | Remove-Item -Force -Recurse -Confirm:$false


$cleanPath   = Join-Path -Path $context.RootFolder -ChildPath "\Dev\Delivery\Atdi.Sdrn\Device\AppServer\Core\BinRelease"
$cleanFilter = "atdi.*.*"
Get-ChildItem -Path $cleanPath -File -Filter $cleanFilter | Remove-Item -Force -Recurse -Confirm:$false


$cleanPath   = Join-Path -Path $context.RootFolder -ChildPath "\Dev\Delivery\Atdi.Sdrn\Device\AppServer\Core\BinRelease\Metadata"
$cleanFilter = "*.*"
Get-ChildItem -Path $cleanPath -Filter $cleanFilter | Remove-Item -Force -Recurse -Confirm:$false



$context.Logger.Info("DeviceServerCore PreBuild Action was executed")
$context.Logger.Info("");