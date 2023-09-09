Build Automator V1.5.1

Build Automator 2 is now under development! If you would like to read more or to join the beta program please see the following for more detail
https://forum.unity.com/threads/wip-build-automator-2-sequel-to-the-ultimate-build-pipeline-solution.926180/

Thank you for purchasing this asset, I hope it finds you well. If you have any questions, suggestions or problems please contact me at:
Email: support@qfsw.co.uk
Discord: https://discord.gg/g8SJ7X6
Issue Tracker: https://bitbucket.org/QFSW/build-automator/issues
Twitter: https://twitter.com/QFSW1024https://twitter.com/QFSW1024

Using a local or remote Unity Cache Server in conjunction with Build Automator is highly recommended for optimal build times

The Asset's UI window is located under Window/Build Automator

Presets
Here you can create Build Automator presets. When you save to a preset, all of your current BA settings are saved to that preset, allowing you to easily switch between different configurations

Preset Pipelining
This feature will allow you to create a sequence of multiple presets that will be executed in sequence, providing you with complete automation capabilities

Dynamic Literals
All dynamic literals that can be used within Build Automator. When dynamic literals are used, they are replaced at build time.
Example: #DATE# --> 10.6.2018

Build Information: The main settings for your project. These should be automatically populated to require minimal setup
Path Type: Whether to use absolute paths or paths relative to root directory path.
Build Name: The base name that will be used for all the outputted executables/directories (Defaults to the product name)
Output Path: The path to the directory to which Build Automator will output the builds; leaving blank will result in the root directory being used
Subfolder per Platform: Whether or not each platform should get its own subfolder within the output folder

Logging: Here you can enable the log file, which will be dumped with post build information on completion

Ignore Scene Settings: Ignores the scene settings chosen in the Build Automator window and uses the scenes selected in Unity's built in Build Settings window instead
Import Editor Scene List: Imports the scene list from the editor's built in Build Settings window
Main Scene: The main scene or scene 0 in the build
Extra Scene Count: Number of extra scenes to include in the build. Changing it to anything greater than 0 will allow you to enter the additional scenes

Build Targets
Here you can enable and disable the targets to which you would like to build, as well as switching between platforms and viewing the build progress
Additionally, you can customise the prefix and suffic, as well as chose the scripting backend

Build Settings
Here you can change the settings available through Unity's built in Build Settings Dialogue
Only those that are applicable to your current selection of build targets will be accessible

Scripting Define Symbols (Preprocessors)
Here you can define what Scripting Define Symbols or Preprocessors to include for different build targets
Universal applies to all builds
Development only applies if development builds are enabled

Batch/Shell Scripts
Here you can choose external batch or shell scripts to be executed after its respective build targets
Universal will be run at the end of the entire build workflow

Other Settings
Revert Platform: Whether or not to return to the target platform prior to clicking 'build'
Save on Build: Automatically saves the current scene if it will be included in the builds

The different build platforms will export to the following directories/files (by default, assuming no prefix and suffix is set)
Windows 64 Bit: Name.exe, Name_Data
Windows 32 Bit: Name.exe, Name_Data
macOS: Name.app
Mac 64 Bit: Name.app
Mac 32 Bit: Name.app
Mac Universal: Namei.app
Linux 64 Bit: Name.x86_64, Name_Data
Linux 32 Bit: Name.x86, Name_Data
Linux Universal: Name.x86, Name_Data
Windows Store: Name
UWP: Name
iOS: Name
Android: Name.apk
Blackberry: Name
Blackberry: Name
tvOS: Name
Tizen: Name.tpk
Samsung TV: Name
Web Player: Name
WebGL: Name
PS3: Name
PS4: Name
PS Vita: Name
Xbox 360: Name
Xbox One: Name
Wii U: Name
3DS: Name
Switch: Name
Facebook: Name

The different subfolders generated if enabled
Windows 64 Bit: Windows64
Windows 32 Bit: Windows32
macOS: macOS
Mac 64 Bit: Mac64
Mac 32 Bit: Mac32
Mac Universal: MacUniversal
Linux 64 Bit: Linux64
Linux 32 Bit: Linux32
Linux Universal: LinuxUniversal
Windows Store: WSA
UWP: UWP
iOS: iOS
Android: Android
Blackberry: Blackberry
Blackberry: Blackberry10
tvOS: tvOS
Tizen: Tizen
Samsung TV: SamsungTV
Web Player: WebPlayer
WebGL: WebGL
PS3: PS3
PS4: PS4
PS Vita: PSVita
Xbox 360: Xbox360
Xbox One: XboxOne
Wii U: Wii U
3DS: 3DS
Switch: Switch
Facebook: Facebook
