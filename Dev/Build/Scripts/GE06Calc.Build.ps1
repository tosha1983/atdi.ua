Param ($context)

### 01. Сборка проекта

# GE06Calc

$context.Logger.Info("GE06Calc Build Action executing")


$context.Build("\Dev\Build\Atdi.Build.Icsm.Plugins.GE06Calc\Atdi.Build.Icsm.Plugins.GE06Calc.csproj" );

$context.Logger.Info("GE06Calc Build Action was executed")
$context.Logger.Info("");

return $true;
