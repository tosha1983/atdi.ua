Param ($context)

### 01. Чистим целевой каталог доставки

# AggregationServer

$context.Logger.Info("AggregationServer PreBuild Action executing")



$cleanPath   = Join-Path -Path $context.RootFolder -ChildPath "\Dev\Delivery\Atdi.Sdrn\Server\Components\AggregationServerRole\BinDebug"
$cleanFilter = "atdi.*.*"
Get-ChildItem -Path $cleanPath -File -Filter $cleanFilter | Remove-Item -Recurse -Force -Confirm:$false


$cleanPath   = Join-Path -Path $context.RootFolder -ChildPath "\Dev\Delivery\Atdi.Sdrn\Server\Components\AggregationServerRole\BinRelease"
$cleanFilter = "atdi.*.*"
Get-ChildItem -Path $cleanPath -File -Filter $cleanFilter | Remove-Item -Recurse -Force -Confirm:$false


$context.Logger.Info("AggregationServer PreBuild Action was executed")
$context.Logger.Info("");