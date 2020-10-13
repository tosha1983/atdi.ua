Param ($context)

### 01. Копирование методаных

# DeviceServerCore

$context.Logger.Info("DeviceServerCore PosBuild Action executing")


$srcPath = Join-Path -Path $context.RootFolder -ChildPath "\Dev\DataModels\Atdi.DataModels.Sdrn.DeviceServer.Entities\Metadata"
$dstPath = Join-Path -Path $context.RootFolder -ChildPath "\Dev\Delivery\Atdi.Sdrn\Device\AppServer\Core\BinDebug\Metadata"
Get-ChildItem -Path $srcPath | Copy-Item -Destination $dstPath -Recurse

$srcPath = Join-Path -Path $context.RootFolder -ChildPath "\Dev\DataModels\Atdi.DataModels.Sdrn.DeviceServer.Entities\Metadata"
$dstPath = Join-Path -Path $context.RootFolder -ChildPath "\Dev\Delivery\Atdi.Sdrn\Device\AppServer\Core\BinRelease\Metadata"
Get-ChildItem -Path $srcPath | Copy-Item -Destination $dstPath -Recurse


$context.Logger.Info("DeviceServerCore PosBuild Action was executed")
$context.Logger.Info("");