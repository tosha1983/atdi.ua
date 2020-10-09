cls

### 00. Путь к размещению сборки

$sourceRootFolder = "E:\Build"


### 01. Очистка каталога от предыдущих файлов

$gitPath = $sourceRootFolder + "\.git"
Get-ChildItem -Path $gitPath -ReadOnly -Recurse | Remove-Item -Verbose -Recurse -Force -Confirm:$false
Get-ChildItem -Path $gitPath -Hidden -Recurse | Remove-Item -Verbose -Recurse -Force -Confirm:$false
Get-ChildItem -Path $sourceRootFolder -Recurse | Remove-Item -Verbose -Recurse -Force -Confirm:$false
Get-ChildItem -Path $sourceRootFolder -Hidden -Recurse | Remove-Item -Verbose -Recurse -Force -Confirm:$false


### 02. Клонирование репозитария

$reposPath = "https://kovpak.visualstudio.com/atdi.ua/_git/atdi.ua"
cd $sourceRootFolder
git --version
git init 
git remote add origin $reposPath
git remote -v
git config gc.auto 0
git fetch --force --tags --prune --progress origin
git checkout --progress --force develop



### 03. Востанавливаем NuGet-пакеты с репозитария

Set-Alias nuget E:\nuget.exe;

$solutionPath = $sourceRootFolder + "\Dev\atdi.sdrn.web.sln"
Write-Host "NuGet pakage restoring '$solutionPath'" -ForegroundColor Yellow
nuget restore $solutionPath

$solutionPath = $sourceRootFolder + "\Dev\atdi.sdrn.deviceserver.sln"
Write-Host "NuGet pakage restoring '$solutionPath'" -ForegroundColor Yellow
nuget restore $solutionPath

$solutionPath = $sourceRootFolder + "\Dev\atdi.sdrn.icsm.plugin.sln"
Write-Host "NuGet pakage restoring '$solutionPath'" -ForegroundColor Yellow
nuget restore $solutionPath

$solutionPath = $sourceRootFolder + "\Dev\atdi.sdrn.icsm.plugin.calcserver.sln"
Write-Host "NuGet pakage restoring '$solutionPath'" -ForegroundColor Yellow
nuget restore $solutionPath

$solutionPath = $sourceRootFolder + "\Dev\atdi.soa.webquery.sln"
Write-Host "NuGet pakage restoring '$solutionPath'" -ForegroundColor Yellow
nuget restore $solutionPath



### 04. Чистим целевой каталог доставки

# CalcServer

$cleanPath   = $sourceRootFolder + "\Dev\Delivery\Atdi.Sdrn\CalcServer\BinDebug"
$cleanFilter = "atdi.*.*"
Get-ChildItem -Path $cleanPath -File -Filter $cleanFilter | Remove-Item -Verbose -Recurse -Force -Confirm:$false

$cleanPath   = $sourceRootFolder + "\Dev\Delivery\Atdi.Sdrn\CalcServer\BinDebug\Metadata"
$cleanFilter = "*.*"
Get-ChildItem -Path $cleanPath -Filter $cleanFilter | Remove-Item -Verbose -Force -Recurse -Confirm:$false

$cleanPath   = $sourceRootFolder + "\Dev\Delivery\Atdi.Sdrn\CalcServer\BinDebug\InfocenterMetadata"
$cleanFilter = "*.*"
Get-ChildItem -Path $cleanPath -Filter $cleanFilter | Remove-Item -Verbose -Force -Recurse -Confirm:$false


$cleanPath   = $sourceRootFolder + "\Dev\Delivery\Atdi.Sdrn\CalcServer\BinRelease"
$cleanFilter = "atdi.*.*"
Get-ChildItem -Path $cleanPath -File -Filter $cleanFilter | Remove-Item -Verbose -Force -Recurse -Confirm:$false

$cleanPath   = $sourceRootFolder + "\Dev\Delivery\Atdi.Sdrn\CalcServer\BinRelease\Metadata"
$cleanFilter = "*.*"
Get-ChildItem -Path $cleanPath -Filter $cleanFilter | Remove-Item -Verbose -Force -Recurse -Confirm:$false

$cleanPath   = $sourceRootFolder + "\Dev\Delivery\Atdi.Sdrn\CalcServer\BinRelease\InfocenterMetadata"
$cleanFilter = "*.*"
Get-ChildItem -Path $cleanPath -Filter $cleanFilter | Remove-Item -Verbose -Force -Recurse -Confirm:$false



### 05.Подготовка к сборке

$msbuild = "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\msbuild.exe" 
# $msbuild = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild.exe"



### 06. Сборка проекта

# CalcServer

$projectPath  = $sourceRootFolder + "\Dev\Build\Atdi.Build.Sdrn.CalcServer\Atdi.Build.Sdrn.CalcServer.csproj" 

Write-Host "Debug version building '$projectPath'" -ForegroundColor Yellow
& $msbuild $projectPath /t:Rebuild  /p:Configuration="Debug" /p:Platform="AnyCpu"

Write-Host "Release building '$projectPath'" -ForegroundColor Yellow
 & $msbuild $projectPath /t:Rebuild  /p:Configuration="Release" /p:Platform="AnyCpu"



### 07. Копирование методаных

# CalcServer

$srcPath = $sourceRootFolder + "\Dev\DataModels\Atdi.DataModels.Sdrn.CalcServer.Entities\Metadata"
$dstPath = $sourceRootFolder + "\Dev\Delivery\Atdi.Sdrn\CalcServer\BinDebug\Metadata"
Get-ChildItem -Path $srcPath | Copy-Item -Destination $dstPath

$srcPath = $sourceRootFolder + "\Dev\DataModels\Atdi.DataModels.Sdrn.Infocenter.Entities\Metadata"
$dstPath = $sourceRootFolder + "\Dev\Delivery\Atdi.Sdrn\CalcServer\BinDebug\InfocenterMetadata"
Get-ChildItem -Path $srcPath | Copy-Item -Destination $dstPath

$srcPath = $sourceRootFolder + "\Dev\DataModels\Atdi.DataModels.Sdrn.CalcServer.Entities\Metadata"
$dstPath = $sourceRootFolder + "\Dev\Delivery\Atdi.Sdrn\CalcServer\BinRelease\Metadata"
Get-ChildItem -Path $srcPath | Copy-Item -Destination $dstPath

$srcPath = $sourceRootFolder + "\Dev\DataModels\Atdi.DataModels.Sdrn.Infocenter.Entities\Metadata"
$dstPath = $sourceRootFolder + "\Dev\Delivery\Atdi.Sdrn\CalcServer\BinRelease\InfocenterMetadata"
Get-ChildItem -Path $srcPath | Copy-Item -Destination $dstPath

### 08. Копирование артифактов сборки в нужное место