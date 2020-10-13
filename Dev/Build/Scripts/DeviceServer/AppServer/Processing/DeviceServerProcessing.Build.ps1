Param ($context)

### 01. Сборка проекта

# DeviceServerClientAPI

$context.Logger.Info("DeviceServerProcessing Build Action executing")


$context.Build("\Dev\Build\Atdi.Build.Sdrn.DeviceServer.Processing\Atdi.Build.Sdrn.DeviceServer.Processing.csproj" );

$context.Logger.Info("DeviceServerProcessing Build Action was executed")
$context.Logger.Info("");

return $true;
