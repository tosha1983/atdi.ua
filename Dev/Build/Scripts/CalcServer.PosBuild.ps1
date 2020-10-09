Param ($context)

### 07. Копирование методаных

# CalcServer

$context.Logger.Info("CalcServer PosBuild Action executing")

$srcPath = Join-Path -Path $context.RootFolder -ChildPath "\Dev\DataModels\Atdi.DataModels.Sdrn.CalcServer.Entities\Metadata"
$dstPath = Join-Path -Path $context.RootFolder -ChildPath "\Dev\Delivery\Atdi.Sdrn\CalcServer\BinDebug\Metadata"
Get-ChildItem -Path $srcPath | Copy-Item -Destination $dstPath

$srcPath = Join-Path -Path $context.RootFolder -ChildPath "\Dev\DataModels\Atdi.DataModels.Sdrn.Infocenter.Entities\Metadata"
$dstPath = Join-Path -Path $context.RootFolder -ChildPath "\Dev\Delivery\Atdi.Sdrn\CalcServer\BinDebug\InfocenterMetadata"
Get-ChildItem -Path $srcPath | Copy-Item -Destination $dstPath

$srcPath = Join-Path -Path $context.RootFolder -ChildPath "\Dev\DataModels\Atdi.DataModels.Sdrn.CalcServer.Entities\Metadata"
$dstPath = Join-Path -Path $context.RootFolder -ChildPath "\Dev\Delivery\Atdi.Sdrn\CalcServer\BinRelease\Metadata"
Get-ChildItem -Path $srcPath | Copy-Item -Destination $dstPath

$srcPath = Join-Path -Path $context.RootFolder -ChildPath "\Dev\DataModels\Atdi.DataModels.Sdrn.Infocenter.Entities\Metadata"
$dstPath = Join-Path -Path $context.RootFolder -ChildPath "\Dev\Delivery\Atdi.Sdrn\CalcServer\BinRelease\InfocenterMetadata"
Get-ChildItem -Path $srcPath | Copy-Item -Destination $dstPath

$context.Logger.Info("CalcServer PosBuild Action was executed")
$context.Logger.Info("");