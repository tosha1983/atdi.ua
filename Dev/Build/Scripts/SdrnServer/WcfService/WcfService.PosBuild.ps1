Param ($context)

### 01. Копирование методаных

# WcfService

$context.Logger.Info("WcfService PosBuild Action executing")



$srcPath = Join-Path -Path $context.RootFolder -ChildPath "\Dev\DataModels\Atdi.DataModels.Sdrn.DeviceServer.Entities\Metadata"
$dstPath = Join-Path -Path $context.RootFolder -ChildPath "\Dev\Delivery\Atdi.Sdrn\Device\WcfService\BinDebug\Metadata"
Get-ChildItem -Path $srcPath | Copy-Item -Destination $dstPath -Recurse

$srcPath = Join-Path -Path $context.RootFolder -ChildPath "\Dev\DataModels\Atdi.DataModels.Sdrn.DeviceServer.Entities\Metadata"
$dstPath = Join-Path -Path $context.RootFolder -ChildPath "\Dev\Delivery\Atdi.Sdrn\Device\WcfService\BinRelease\Metadata"
Get-ChildItem -Path $srcPath | Copy-Item -Destination $dstPath -Recurse


$context.Logger.Info("WcfService PosBuild Action was executed")
$context.Logger.Info("");