%systemroot%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe /t:build /v:m /nologo /p:Configuration=Debug.Net40 ObjectTestHelpers\ObjectTestHelpers.csproj
%systemroot%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe /t:build /v:m /nologo /p:Configuration=Debug.Net45 ObjectTestHelpers\ObjectTestHelpers.csproj
nuget pack ObjectTestHelpers.nuspec
nuget push ObjectTestHelpers.*.nupkg
del ObjectTestHelpers.*.nupkg
pause