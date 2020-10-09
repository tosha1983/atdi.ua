Param ($context)

$context.Logger.Info("Started repository cloning")
$context.Logger.Info("    path: '$($context.ReposPath)")

cd $context.RootFolder
git --version
git init 
git remote add origin $context.ReposPath
git remote -v
git config gc.auto 0
git fetch --force --tags --prune --progress origin
git checkout --progress --force develop

$context.Logger.Info("Repository '$($context.ReposPath)' was cloned")
$context.logger.Info("");

return $true;