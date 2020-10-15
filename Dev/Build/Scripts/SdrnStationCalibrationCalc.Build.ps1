Param ($context)

### 01. Сборка проекта

# SdrnStationCalibrationCalc

$context.Logger.Info("SdrnStationCalibrationCalc Build Action executing")


$context.Build("\Dev\Build\Atdi.Build.Icsm.Plugins.StationCalibrationCalcClient\Atdi.Build.Icsm.Plugins.StationCalibrationCalcClient.csproj" );

$context.Logger.Info("SdrnStationCalibrationCalc Build Action was executed")
$context.Logger.Info("");

return $true;
