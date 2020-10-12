Param ($context)

### 04. Чистим целевой каталог доставки

# CalcServer

$context.Logger.Info("CalcServer PreBuild Action executing")

$cleanPath   = Join-Path -Path $context.RootFolder -ChildPath "\Dev\Delivery\Atdi.Sdrn\CalcServer\BinDebug"
$cleanFilter = "atdi.*.*"
Get-ChildItem -Path $cleanPath -File -Filter $cleanFilter | Remove-Item -Recurse -Force -Confirm:$false

$cleanPath   = Join-Path -Path $context.RootFolder -ChildPath "\Dev\Delivery\Atdi.Sdrn\CalcServer\BinDebug\Metadata"
$cleanFilter = "*.*"
Get-ChildItem -Path $cleanPath -Filter $cleanFilter | Remove-Item -Force -Recurse -Confirm:$false

$cleanPath   = Join-Path -Path $context.RootFolder -ChildPath "\Dev\Delivery\Atdi.Sdrn\CalcServer\BinDebug\InfocenterMetadata"
$cleanFilter = "*.*"
Get-ChildItem -Path $cleanPath -Filter $cleanFilter | Remove-Item -Force -Recurse -Confirm:$false

$cleanPath   = Join-Path -Path $context.RootFolder -ChildPath "\Dev\Delivery\Atdi.Sdrn\CalcServer\BinRelease"
$cleanFilter = "atdi.*.*"
Get-ChildItem -Path $cleanPath -File -Filter $cleanFilter | Remove-Item -Force -Recurse -Confirm:$false

$cleanPath   = Join-Path -Path $context.RootFolder -ChildPath "\Dev\Delivery\Atdi.Sdrn\CalcServer\BinRelease\Metadata"
$cleanFilter = "*.*"
Get-ChildItem -Path $cleanPath -Filter $cleanFilter | Remove-Item -Force -Recurse -Confirm:$false

$cleanPath   = Join-Path -Path $context.RootFolder -ChildPath "\Dev\Delivery\Atdi.Sdrn\CalcServer\BinRelease\InfocenterMetadata"
$cleanFilter = "*.*"
Get-ChildItem -Path $cleanPath -Filter $cleanFilter | Remove-Item -Force -Recurse -Confirm:$false

$context.Logger.Info("CalcServer PreBuild Action was executed")
$context.Logger.Info("");