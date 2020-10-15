Param ($context)

### 01. Сборка проекта

# SdrnCalcServerClient

$context.Logger.Info("SdrnCalcServerClient Build Action executing")


$context.Build("\Dev\Build\Atdi.Build.Icsm.Plugins.SdrnCalcServerClient\Atdi.Build.Icsm.Plugins.SdrnCalcServerClient.csproj" );

$context.Logger.Info("SdrnCalcServerClient Build Action was executed")
$context.Logger.Info("");

return $true;
