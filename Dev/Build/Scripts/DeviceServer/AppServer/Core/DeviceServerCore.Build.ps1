Param ($context)

### 01. Сборка проекта

# DeviceServerCore

$context.Logger.Info("DeviceServerProcessing Build Action executing")


$context.Build("\Dev\Build\Atdi.Build.Sdrn.DeviceServer.Core\Atdi.Build.Sdrn.DeviceServer.Core.csproj" );

$context.Logger.Info("DeviceServerCore Build Action was executed")
$context.Logger.Info("");

return $true;
