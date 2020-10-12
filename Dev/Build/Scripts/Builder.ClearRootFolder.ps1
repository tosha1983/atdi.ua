Param ($context)


### 01. Очистка каталога от предыдущих файлов

$gitPath = $sourceRootFolder + "\.git"
Get-ChildItem -Path $context.GitFolder -ReadOnly -Recurse | Remove-Item  -Recurse -Force -Confirm:$false
Get-ChildItem -Path $context.GitFolder -Hidden -Recurse | Remove-Item -Recurse -Force -Confirm:$false
Get-ChildItem -Path $context.RootFolder -Recurse -Exclude "build.log" | Remove-Item -Recurse -Force -Confirm:$false
Get-ChildItem -Path $context.RootFolder -Hidden -Exclude "build.log" -Recurse | Remove-Item -Recurse -Force -Confirm:$false

$context.Logger.Info("Building folder was cleaned")
$logger.Info("");

return $true;