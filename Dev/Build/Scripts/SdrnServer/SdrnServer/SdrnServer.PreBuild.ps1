Param ($context)

### 01. Чистим целевой каталог доставки

# SdrnServer

$context.Logger.Info("SdrnServer PreBuild Action executing")

$cleanPath   = Join-Path -Path $context.RootFolder -ChildPath "\Dev\Delivery\Atdi.Sdrn\Server\BinDebug"
$cleanFilter = "atdi.*.*"
Get-ChildItem -Path $cleanPath -File -Filter $cleanFilter | Remove-Item -Recurse -Force -Confirm:$false

$cleanPath   = Join-Path -Path $context.RootFolder -ChildPath "\Dev\Delivery\Atdi.Sdrn\Server\BinDebug\Metadata"
$cleanFilter = "*.*"
Get-ChildItem -Path $cleanPath -Filter $cleanFilter | Remove-Item -Force -Recurse -Confirm:$false


$cleanPath   = Join-Path -Path $context.RootFolder -ChildPath "\Dev\Delivery\Atdi.Sdrn\Server\BinRelease"
$cleanFilter = "atdi.*.*"
Get-ChildItem -Path $cleanPath -File -Filter $cleanFilter | Remove-Item -Force -Recurse -Confirm:$false

$cleanPath   = Join-Path -Path $context.RootFolder -ChildPath "\Dev\Delivery\Atdi.Sdrn\Server\BinRelease\Metadata"
$cleanFilter = "*.*"
Get-ChildItem -Path $cleanPath -Filter $cleanFilter | Remove-Item -Force -Recurse -Confirm:$false



$cleanPath   = Join-Path -Path $context.RootFolder -ChildPath "\Dev\Delivery\Atdi.Sdrn\Server\Components\DataLayer\Oracle"
$cleanFilter = "atdi.*.*"
Get-ChildItem -Path $cleanPath -File -Filter $cleanFilter | Remove-Item -Recurse -Force -Confirm:$false



$context.Logger.Info("SdrnServer PreBuild Action was executed")
$context.Logger.Info("");