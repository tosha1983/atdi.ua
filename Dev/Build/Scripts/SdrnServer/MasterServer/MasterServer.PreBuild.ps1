Param ($context)

### 01. Чистим целевой каталог доставки

# SdrnServer

$context.Logger.Info("MasterServer PreBuild Action executing")



$cleanPath   = Join-Path -Path $context.RootFolder -ChildPath "\Dev\Delivery\Atdi.Sdrn\Server\Components\MasterServerRole\BinDebug"
$cleanFilter = "atdi.*.*"
Get-ChildItem -Path $cleanPath -File -Filter $cleanFilter | Remove-Item -Recurse -Force -Confirm:$false


$cleanPath   = Join-Path -Path $context.RootFolder -ChildPath "\Dev\Delivery\Atdi.Sdrn\Server\Components\MasterServerRole\BinRelease"
$cleanFilter = "atdi.*.*"
Get-ChildItem -Path $cleanPath -File -Filter $cleanFilter | Remove-Item -Recurse -Force -Confirm:$false



$context.Logger.Info("MasterServer PreBuild Action was executed")
$context.Logger.Info("");