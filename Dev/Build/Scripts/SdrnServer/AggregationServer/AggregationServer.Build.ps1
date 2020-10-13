Param ($context)

### 01. Сборка проекта

# AggregationServer

$context.Logger.Info("AggregationServer Build Action executing")


$context.Build("\Dev\Build\Atdi.Build.Sdrn.AggregationServer\Atdi.Build.Sdrn.AggregationServer.csproj" );

$context.Logger.Info("AggregationServer Build Action was executed")
$context.Logger.Info("");

return $true;
