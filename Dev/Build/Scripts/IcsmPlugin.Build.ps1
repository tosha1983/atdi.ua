Param ($context)

### 01. Сборка проекта

# SdrnStationCalibrationCalc

$context.Logger.Info("IcsmPlugin Build Action executing")


$context.Build("\Dev\Build\Atdi.Build.Sdrn.Plugin\Atdi.Build.Sdrn.Plugin.csproj" );

$context.Logger.Info("IcsmPlugin Build Action was executed")
$context.Logger.Info("");

return $true;
