Param ($context)

### 01. Копирование методаных

# SdrnServer

$context.Logger.Info("SdrnServer PosBuild Action executing")

$srcPath = Join-Path -Path $context.RootFolder -ChildPath "\Dev\DataModels\Atdi.DataModels.Sdrns.Server.Entities\Metadata"
$dstPath = Join-Path -Path $context.RootFolder -ChildPath "\Dev\Delivery\Atdi.Sdrn\Server\BinDebug\Metadata"
Get-ChildItem -Path $srcPath | Copy-Item -Destination $dstPath -Recurse

$srcPath = Join-Path -Path $context.RootFolder -ChildPath "\Dev\DataModels\Atdi.DataModels.Sdrns.Server.Entities\Metadata"
$dstPath = Join-Path -Path $context.RootFolder -ChildPath "\Dev\Delivery\Atdi.Sdrn\Server\BinRelease\Metadata"
Get-ChildItem -Path $srcPath | Copy-Item -Destination $dstPath -Recurse


$context.Logger.Info("SdrnServer PosBuild Action was executed")
$context.Logger.Info("");