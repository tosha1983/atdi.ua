Param ($context)

### 01. Сборка проекта

# SdrnServer

$context.Logger.Info("SdrnServer Build Action executing")


$context.Build("\Dev\Build\Atdi.Build.Sdrn.Server.v2\Atdi.Build.Sdrn.Server.v2.csproj" );

$context.Logger.Info("SdrnServer Build Action was executed")
$context.Logger.Info("");

return $true;
