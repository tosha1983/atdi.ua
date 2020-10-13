Param ($context)

### 01. Сборка проекта

# MasterServer

$context.Logger.Info("MasterServer Build Action executing")


$context.Build("\Dev\Build\Atdi.Build.Sdrn.MasterServer\Atdi.Build.Sdrn.MasterServer.csproj" );

$context.Logger.Info("MasterServer Build Action was executed")
$context.Logger.Info("");

return $true;
