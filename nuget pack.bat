nuget pack ObjectTestHelpers\ObjectTestHelpers.csproj -Prop Configuration=Release
nuget push ObjectTestHelpers.*.nupkg
del ObjectTestHelpers.*.nupkg
pause