Param ($context)

### 01. Сборка проекта

# WcfService

$context.Logger.Info("WcfService Build Action executing")


$context.Build("\Dev\Build\Atdi.Build.Sdrn.Device.WcfService\Atdi.Build.Sdrn.Device.WcfService.csproj" );

$context.Logger.Info("WcfService Build Action was executed")
$context.Logger.Info("");

return $true;
