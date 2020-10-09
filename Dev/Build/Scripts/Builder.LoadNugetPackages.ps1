Param ($context)

### 03. Востанавливаем NuGet-пакеты с репозитария

$nugetPath = $context.RootFolder + "\nuget.exe"

[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
Invoke-WebRequest "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe" -OutFile $nugetPath

Set-Alias nuget $nugetPath;

$solutionPath = $context.RootFolder + "\Dev\atdi.sdrn.web.sln"
$context.Logger.Info("NuGet package restoring")
$context.Logger.Info("    Solution: $($solutionPath)")
nuget restore $solutionPath
$context.Logger.Info("NuGet package was restored")
$context.logger.Info("");

$solutionPath = $context.RootFolder + "\Dev\atdi.sdrn.deviceserver.sln"
$context.Logger.Info("NuGet package restoring")
$context.Logger.Info("    Solution: $($solutionPath)")
nuget restore $solutionPath
$context.Logger.Info("NuGet package was restored")
$context.logger.Info("");

$solutionPath = $context.RootFolder + "\Dev\atdi.sdrn.icsm.plugin.sln"
$context.Logger.Info("NuGet package restoring")
$context.Logger.Info("    Solution: $($solutionPath)")
nuget restore $solutionPath
$context.Logger.Info("NuGet package was restored")
$context.logger.Info("");

$solutionPath = $context.RootFolder + "\Dev\atdi.sdrn.icsm.plugin.calcserver.sln"
$context.Logger.Info("NuGet package restoring")
$context.Logger.Info("    Solution: $($solutionPath)")
nuget restore $solutionPath
$context.Logger.Info("NuGet package was restored")
$context.logger.Info("");

$solutionPath = $context.RootFolder + "\Dev\atdi.soa.webquery.sln"
$context.Logger.Info("NuGet package restoring")
$context.Logger.Info("    Solution: $($solutionPath)")
nuget restore $solutionPath
$context.Logger.Info("NuGet package was restored")
$context.Logger.Info("");

return $true;