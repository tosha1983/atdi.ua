Param ($sourceRootFolder, $scriptsPath, $reposPath)

function Create-LoggerObject($fileName, $path)
{
    $logFileName = Join-Path -Path $path -ChildPath $fileName

    $loggerObject = New-Object -TypeName PSObject -Property `
    @{
        LogFileName = $logFileName
    }

    $loggerObject | Add-Member -MemberType ScriptMethod -Name Info -Value `
    {
        Param($message)
        $time = Get-Date
        $msg = "$Time Info - $message"
        $msg | out-file $this.LogFileName -append
        Write-Host $msg
    }

    $loggerObject | Add-Member -MemberType ScriptMethod -Name Error -Value `
    {
        Param($message)
        $time = Get-Date
        $msg = "$Time Error - $message"
        $msg | out-file $this.LogFileName -append
        Write-Host $msg -ForegroundColor Red
    }

    $loggerObject | Add-Member -MemberType ScriptMethod -Name Warning -Value `
    {
        Param($message)
        $time = Get-Date
        $msg = "$Time Warning - $message"
        $msg | out-file $this.LogFileName -append
        Write-Host $msg -ForegroundColor DarkRed
    }

	$loggerObject | Add-Member -MemberType ScriptMethod -Name Exception -Value `
    {
        Param($result, $title)
        $this.WriteError($title)
        $this.WriteError("  Message      : " + $result.Message)
        $this.WriteError("  StackTrace   : " + $result.Detail.StackTrace)
        $this.WriteError("  Target       : " + $result.Detail.Target)
        $this.WriteError("  Source       : " + $result.Detail.Source)
        $this.WriteError("  Description  : " + $result.Detail.Description)
    }

    return $loggerObject
}

cd $sourceRootFolder

$logger = Create-LoggerObject -fileName "build.log" -path $sourceRootFolder
$logger.Info("Init logger");
$logger.Info("");

$contextObject = New-Object -TypeName PSObject -Property `
@{
    ScriptsPath = $scriptsPath
    RootFolder = $sourceRootFolder
    GitFolder = $sourceRootFolder + "\.git"
    ReposPath = $reposPath
    State = "InProcess"
	Logger = $logger
}

$contextObject | Add-Member -MemberType ScriptMethod -Name Execute -Value `
{
    Param($scriptName)

    $executionScript = Join-Path -Path $this.ScriptsPath -ChildPath $scriptName
    $result = . $executionScript -context $this
    return $result;
}

$contextObject | Add-Member -MemberType ScriptMethod -Name Build -Value `
{
    Param($path)

    $msbuild = "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\msbuild.exe" 


    $projectPath  = Join-Path -Path $this.RootFolder -ChildPath $path

    Write-Host "    ... debug building '$projectPath'" -ForegroundColor Yellow
    & $msbuild $projectPath /t:Rebuild  /p:Configuration="Debug" /p:Platform="AnyCpu"

    Write-Host "    ... release building '$projectPath'" -ForegroundColor Yellow
     & $msbuild $projectPath /t:Rebuild  /p:Configuration="Release" /p:Platform="AnyCpu"

}

$logger.Info("Created context object");
$logger.Info("    RootFolder:  $($contextObject.RootFolder)");
$logger.Info("    GitFolder :  $($contextObject.GitFolder)");
$logger.Info("");


return $contextObject;