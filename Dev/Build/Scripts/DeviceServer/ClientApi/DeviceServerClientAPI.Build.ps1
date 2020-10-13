Param ($context)

### 01. Сборка проекта

# DeviceServerClientAPI

$context.Logger.Info("DeviceServerClientAPI Build Action executing")


$context.Build("\Dev\Build\Atdi.Build.Sdrn.Device.ClientAPI\Atdi.Build.Sdrn.Device.ClientAPI.csproj" );

$context.Logger.Info("DeviceServerClientAPI Build Action was executed")
$context.Logger.Info("");

return $true;
