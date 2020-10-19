Param ($context)

### 01. Чистим целевой каталог доставки

# GE06Calc

$context.Logger.Info("GE06Calc PreBuild Action executing")



$cleanPath   = Join-Path -Path $context.RootFolder -ChildPath "\Dev\Delivery\Atdi.Sdrn\Client\GE06Calc\BinDebug"
$cleanFilter = "atdi.*.*"
Get-ChildItem -Path $cleanPath -File -Filter $cleanFilter | Remove-Item -Recurse -Force -Confirm:$false


$cleanPath   = Join-Path -Path $context.RootFolder -ChildPath "\Dev\Delivery\Atdi.Sdrn\Client\GE06Calc\BinRelease"
$cleanFilter = "atdi.*.*"
Get-ChildItem -Path $cleanPath -File -Filter $cleanFilter | Remove-Item -Recurse -Force -Confirm:$false



$context.Logger.Info("GE06Calc PreBuild Action was executed")
$context.Logger.Info("");