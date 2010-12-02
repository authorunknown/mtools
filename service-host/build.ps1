param([switch]$release)

if ($release) {
    $config = 'Release'
} else {
    $config = 'Debug'
}
$wc = split-path $MyInvocation.MyCommand.Definition -Parent
C:\windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe `
    "$wc\src\src.sln" `
    "/p:Configuration=$config" `
    "/p:OutDir=$wc\build\$config\" `
    "/t:Build" `
    "/verbosity:minimal" `
    "/fl" `
    "/flp:Verbosity=detailed"