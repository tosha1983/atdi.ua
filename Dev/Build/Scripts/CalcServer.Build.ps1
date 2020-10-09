Param ($context)

### 06. Сборка проекта

# CalcServer

$context.Logger.Info("CalcServer Build Action executing")


$context.Build("\Dev\Build\Atdi.Build.Sdrn.CalcServer\Atdi.Build.Sdrn.CalcServer.csproj" );

$context.Logger.Info("CalcServer Build Action was executed")
$context.Logger.Info("");

return $true;
