Param ($context)

### 01. Сборка проекта

# DeviceServerAdapter

$context.Logger.Info("DeviceServerAdapter Build Action executing")


$context.Build("\Dev\Build\Atdi.Build.Sdrn.DeviceServer.Adapters\Atdi.Build.Sdrn.DeviceServer.Adapters.csproj" );

$context.Logger.Info("DeviceServerAdapter Build Action was executed")
$context.Logger.Info("");

return $true;
