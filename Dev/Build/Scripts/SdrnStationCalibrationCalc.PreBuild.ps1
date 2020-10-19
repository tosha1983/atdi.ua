Param ($context)

### 01. Чистим целевой каталог доставки

# SdrnStationCalibrationCalc

$context.Logger.Info("SdrnStationCalibrationCalc PreBuild Action executing")



$cleanPath   = Join-Path -Path $context.RootFolder -ChildPath "\Dev\Delivery\Atdi.Sdrn\Client\SdrnCalcServerClient\BinDebug"
$cleanFilter = "atdi.*.*"
Get-ChildItem -Path $cleanPath -File -Filter $cleanFilter | Remove-Item -Recurse -Force -Confirm:$false


$cleanPath   = Join-Path -Path $context.RootFolder -ChildPath "\Dev\Delivery\Atdi.Sdrn\Client\SdrnCalcServerClient\BinRelease"
$cleanFilter = "atdi.*.*"
Get-ChildItem -Path $cleanPath -File -Filter $cleanFilter | Remove-Item -Recurse -Force -Confirm:$false



$context.Logger.Info("SdrnStationCalibrationCalc PreBuild Action was executed")
$context.Logger.Info("");