# test.ps1 - Run the offline test suite (no game, no Unity).
$ErrorActionPreference = "Stop"
dotnet test "$PSScriptRoot\HandOfFate2Access.Tests\HandOfFate2Access.Tests.csproj" @args
exit $LASTEXITCODE
