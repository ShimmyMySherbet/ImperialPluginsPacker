# ImperialPluginsPacker
An Unturned plugin packer for ImperialPlugins.Net

Packs plugins into a zip file in the correct format for uploading to ImperialPlugins. Automatically detects plugin version from assembly info, and ignores unnecessary libraries.

## CLI Usage
`PackForIP [Build Folder] [Platform] {Plugin Binary} {OutputFile}`

## Auto Pack Usage
Post build event for Visual Studio. This will automatically pack the plugin on successful build.

`PackForIP.exe $(ProjectDir)$(OutDir) [platform] [plugin].dll`
