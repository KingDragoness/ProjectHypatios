using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using System;
using System.Reflection;
using System.IO;
using System.Linq;

namespace QFSW.BA
{

#if !UNITY_5_4_OR_NEWER
    //Enum for WebGL Optimisation Level
    /// <summary>The optimisation level to use for WebGL builds.</summary>
    public enum WebOptimisation
    {
        Slow = 1,
        Fast = 2,
        VeryFast = 3
    }
#endif

#if UNITY_2017_1_OR_NEWER
    public enum CompressionOptions
    {
        None = 0,
        LZ4 = 1,
        LZ4HC = 2
    }
#endif

    /// <summary>The Editor Window that manages all the build settings, Platforms and paths and ultimately controls the build pipeline.</summary>
    public partial class BuildAutomator : EditorWindow
    {
#region VariableDeclarations
#pragma warning disable 0618
        /// <summary>The optimal order to perform the builds in.</summary>
        private Dictionary<BuildTarget, int> OptimisedOrder = new Dictionary<BuildTarget, int>()
        {
            { BuildTarget.StandaloneOSXIntel64, 0 },
            { BuildTarget.StandaloneOSXIntel, 1 },
            { BuildTarget.StandaloneWindows64, 3 },
            { BuildTarget.StandaloneWindows, 4 },
            { BuildTarget.StandaloneLinux64, 5 },
            { BuildTarget.StandaloneLinux, 6 },
            { BuildTarget.StandaloneLinuxUniversal, 7 },
            { BuildTarget.WSAPlayer, 8 },
            { BuildTarget.iOS, 9 },
            { BuildTarget.tvOS, 10 },
            { BuildTarget.Android, 11 },
            { BuildTarget.BlackBerry, 12 },
            { BuildTarget.Tizen, 13 },
            { BuildTarget.SamsungTV, 14 },
            { BuildTarget.WebGL, 17 },
            { BuildTarget.PS3, 18 },
            { BuildTarget.PS4, 19 },
            { BuildTarget.PSP2, 20 },
            { BuildTarget.XBOX360, 21 },
            { BuildTarget.XboxOne, 22 },
            { BuildTarget.WiiU, 23 }
        };
#pragma warning restore 0618

        //Paths
        /// <summary>The name of the project/builds.</summary>
        public string BuildName;
        /// <summary>Path to the BuildAutomator directory.</summary>
        private string ScriptPath;
        /// <summary>The path to the root directory of the current project.</summary>
        private string RootPath;
        /// <summary>The (relative/absolute) output path for the builds specified by the user.</summary>
        public string OutputPath;
        /// <summary>The absolute path that all builds will be built to.</summary>
        public string BuildPath;

        private List<Platform> platforms = new List<Platform>();
        private List<DynamicLiteral> dynamicLiterals = new List<DynamicLiteral>();

        //Scenes
        private readonly SceneList sceneList = new SceneList();
        public string[] scenes { get; private set; }

        //Universal Settings
        /// <summary>Enables development builds.</summary>
        public bool Dev;
        /// <summary>Switches back to the original build target after completing the builds.</summary>
        private bool Revert;
        /// <summary>Automatically saves the current scene if it will be included in the builds.</summary>
        private bool SaveOnBuild;
        /// <summary>Run batch scripts after each build.</summary>
        public bool Batch;
        /// <summary>Runs a universal batch script, which will run at the end of the entire build pipeline.</summary>
        private bool UniBatch;
        /// <summary>Gives each Platform its own subfolder within the output folder.</summary>
        public bool Subfolders;
        /// <summary>Path to the universal batch script.</summary>
        private string UniversalBatchPath;
        /// <summary>The scripting define symbols that will be used for development builds.</summary>
        public string DevDefine;
        /// <summary>The scripting define symbols that will be used for all Platforms.</summary>
        public string UniDefine;

        //Presets
        /// <summary>All available user presets.</summary>
        private string[] AvailablePresets = { "None" };
        /// <summary>The currently selected preset.</summary>
        private int CurrentPresetID;

        //Preset pipeline
        /// <summary>If the preset should be reverted after pipeline execution.</summary>
        private bool RevertPipeline;
        private readonly PresetList presetPipelineList = new PresetList();

        //Current job information
        /// <summary>Total number of jobs in the build pipeline.</summary>
        private int Jobs;
        /// <summary>Current job number.</summary>
        private float CurrentJobNumber;
        /// <summary>Progress of build pipeline.</summary>
        private float Progress;
        /// <summary>The current job name.</summary>
        private string CurrentJob;
        /// <summary>Label of current job to display in editor.</summary>
        private string JobLabel;

        /// <summary>If Build Automator is currently building.</summary>
        public bool Building;

        /// <summary>Current PathType</summary>
        private PathType DesiredPathType = PathType.Relative;

        //Current Platform
        /// <summary>Active Platform.</summary>
        public Platform CurrentPlatform;
        /// <summary>Active Platform before initiating the build sequence.</summary>
        private Platform OldPlatform;
        /// <summary>If Build Automator should auto save to the disk when the window is destroyed.</summary>
        private bool AutoSaveDestroy = true;
        /// <summary>If Build Automator should auto save to the disk when the window has lost focus.</summary>
        private bool AutoSaveLoseFocus = true;
#if UNITY_2017_1_OR_NEWER
        /// <summary>Compression type to use for for builds.</summary>
        public CompressionOptions CompressionType { get; private set; }
#endif

#if UNITY_2018_1_OR_NEWER
        //Logging
        /// <summary>Enables log file dumping for the build process.</summary>
        private bool DumpToLog;

        /// <summary>The currently selected path to the log file.</summary>
        private string LogFilePath;

        /// <summary>Current StreamWriter being used to write the log file.</summary>
        public StreamWriter CurrentLogWriter { get; private set; }
#endif
#endregion

#region Initialisation
        //On window enable
        private void OnEnable()
        {
            //Hides any active notification
            RemoveNotification();

            //Finds path to Build Automator
            ScriptPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
            ScriptPath = ScriptPath.Substring(0, ScriptPath.LastIndexOf("/") + 1);
            RootPath = Application.dataPath.Truncate(Application.dataPath.Length - 7).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

            //Populates texture list
            PlatformIcons = new List<Texture>
            {
                AssetDatabase.LoadAssetAtPath<Texture>(ScriptPath + "Icons/Mac.png"),
                AssetDatabase.LoadAssetAtPath<Texture>(ScriptPath + "Icons/Windows.png"),
                AssetDatabase.LoadAssetAtPath<Texture>(ScriptPath + "Icons/Linux.png"),
                AssetDatabase.LoadAssetAtPath<Texture>(ScriptPath + "Icons/WSA.png"),
                AssetDatabase.LoadAssetAtPath<Texture>(ScriptPath + "Icons/iOS.png"),
                AssetDatabase.LoadAssetAtPath<Texture>(ScriptPath + "Icons/Android.png"),
                AssetDatabase.LoadAssetAtPath<Texture>(ScriptPath + "Icons/Blackberry.png"),
                AssetDatabase.LoadAssetAtPath<Texture>(ScriptPath + "Icons/tvOS.png"),
                AssetDatabase.LoadAssetAtPath<Texture>(ScriptPath + "Icons/Tizen.png"),
                AssetDatabase.LoadAssetAtPath<Texture>(ScriptPath + "Icons/SamsungTV.png"),
                AssetDatabase.LoadAssetAtPath<Texture>(ScriptPath + "Icons/WebPlayer.png"),
                AssetDatabase.LoadAssetAtPath<Texture>(ScriptPath + "Icons/WebGL.png"),
                AssetDatabase.LoadAssetAtPath<Texture>(ScriptPath + "Icons/PS3.png"),
                null,
                AssetDatabase.LoadAssetAtPath<Texture>(ScriptPath + "Icons/PS.png"),
                null,
                null,
                AssetDatabase.LoadAssetAtPath<Texture>(ScriptPath + "Icons/Xbox.png"),
                AssetDatabase.LoadAssetAtPath<Texture>(ScriptPath + "Icons/WiiU.png"),
                null,
                AssetDatabase.LoadAssetAtPath<Texture>(ScriptPath + "Icons/3DS.png"),
                AssetDatabase.LoadAssetAtPath<Texture>(ScriptPath + "Icons/Switch.png"),
                AssetDatabase.LoadAssetAtPath<Texture>(ScriptPath + "Icons/FB.png")
            };
            Banner = AssetDatabase.LoadAssetAtPath<Texture>(ScriptPath + "Icons/Banner.png");
            FolderIcon = AssetDatabase.LoadAssetAtPath<Texture>(ScriptPath + "Icons/Folder.png");
            BAIcon = AssetDatabase.LoadAssetAtPath<Texture>(ScriptPath + "Icons/BuildAutomatorIconSmall.png");

#if UNITY_2018_3_OR_NEWER
            titleContent = new GUIContent("Build Automator", "Editor window for Build Automator");
#else
            titleContent = new GUIContent("BA", "Editor window for Build Automator");
#endif

#pragma warning disable 0618
            //Populates Platform list
            platforms = new List<Platform>
            {
                new Platform("Mac 64 Bit", ".app", "Building Mac 64 Bit Standalone Player", BuildTarget.StandaloneOSXIntel64, BuildTargetGroup.Standalone, "Mac64", PlatformIcons[0]),
                new Platform("Mac 32 Bit", ".app", "Building Mac 32 Bit Standalone Player", BuildTarget.StandaloneOSXIntel, BuildTargetGroup.Standalone, "Mac32", PlatformIcons[0]),
                new Platform("Windows 64 Bit", ".exe","Building Windows 64 Bit Standalone Player", BuildTarget.StandaloneWindows64, BuildTargetGroup.Standalone, "Windows64", PlatformIcons[1]),
                new Platform("Windows 32 Bit", ".exe","Building Windows 32 Bit Standalone Player", BuildTarget.StandaloneWindows, BuildTargetGroup.Standalone, "Windows32", PlatformIcons[1]),
                new Platform("Linux 64 Bit", ".x86_64","Building Linux 64 Bit Standalone Player", BuildTarget.StandaloneLinux64, BuildTargetGroup.Standalone, "Linux64", PlatformIcons[2]),
                new Platform("Linux 32 Bit", ".x86","Building Linux 32 Bit Standalone Player", BuildTarget.StandaloneLinux, BuildTargetGroup.Standalone, "Linux32", PlatformIcons[2]),
                new Platform("Linux Universal", ".x86","Building Linux Universal Standalone Player", BuildTarget.StandaloneLinuxUniversal, BuildTargetGroup.Standalone, "LinuxUniversal", PlatformIcons[2]),
                new Platform("Windows Store", "","Building Windows Store Application", BuildTarget.WSAPlayer, BuildTargetGroup.WSA, "WSA", PlatformIcons[1]),
                new Platform("iOS","", "Building iOS Project", BuildTarget.iOS, BuildTargetGroup.iOS, PlatformIcons[4]),
                new Platform("Android", ".apk", "Building Android APK", BuildTarget.Android, BuildTargetGroup.Android, PlatformIcons[5]),
                new Platform("Blackberry", "", "Building Blackberry Project", BuildTarget.BlackBerry, BuildTargetGroup.BlackBerry, PlatformIcons[6]),
                new Platform("tvOS", "", "Building tvOS Project", BuildTarget.tvOS, BuildTargetGroup.tvOS, PlatformIcons[7]),
                new Platform("Tizen", "", "Building Tizen TPK", BuildTarget.Tizen, BuildTargetGroup.Tizen, PlatformIcons[8]),
                new Platform("SamsungTV", "", "Building Samsung TV Project", BuildTarget.SamsungTV, BuildTargetGroup.SamsungTV, PlatformIcons[9]),
                new Platform("WebGL", "", "Building WebGL Project", BuildTarget.WebGL, BuildTargetGroup.WebGL, PlatformIcons[11]),
                new Platform("PS3", "", "Building PS3 Project", BuildTarget.PS3, BuildTargetGroup.PS3, PlatformIcons[14]),
                new Platform("PS4", "", "Building PS4 Project", BuildTarget.PS4, BuildTargetGroup.PS4, PlatformIcons[14]),
                new Platform("PS Vita", "", "Building PS Vita Project", BuildTarget.PSP2, BuildTargetGroup.PSP2, "PSVita", PlatformIcons[14]),
                new Platform("Xbox 360", "", "Building Xbox 360 Project", BuildTarget.XBOX360, BuildTargetGroup.XBOX360, "XB360", PlatformIcons[17]),
                new Platform("Xbox One", "", "Building Xbox One Project", BuildTarget.XboxOne, BuildTargetGroup.XboxOne, "XBOne", PlatformIcons[17]),
                new Platform("Wii U", "", "Building Wii U Project", BuildTarget.WiiU, BuildTargetGroup.WiiU, "WiiU", PlatformIcons[18])

            };
#if UNITY_2017_1_OR_NEWER
            platforms[platforms.FindIndex(x => x.TargetPlatform == BuildTarget.WSAPlayer)] = new Platform("UWP", "_UWP", "Building Universal Windows Program Project", BuildTarget.WSAPlayer, BuildTargetGroup.WSA, "UWP", PlatformIcons[1]);
#if UNITY_2017_3_OR_NEWER
            platforms.RemoveAll(x => x.TargetPlatform == BuildTarget.StandaloneOSX
                                             || x.TargetPlatform == BuildTarget.StandaloneOSXIntel
                                             || x.TargetPlatform == BuildTarget.StandaloneOSXIntel64);
            platforms.Insert(0, new Platform("macOS", ".app", "Building macOS Standalone Player", BuildTarget.StandaloneOSX, BuildTargetGroup.Standalone, "macOS", PlatformIcons[0]));
            OptimisedOrder.Add(BuildTarget.StandaloneOSX, 2);
#if UNITY_2018_1_OR_NEWER
            platforms.RemoveAll(x => x.TargetPlatform == BuildTarget.Tizen);
#endif
#endif
#endif
#if !UNITY_2017_3_OR_NEWER
            OptimisedOrder.Add(BuildTarget.StandaloneOSXUniversal, 2);
            Platforms.Insert(0, new Platform("Mac Universal", "_Uni.app", "Building Mac Universal Standalone Project", BuildTarget.StandaloneOSXUniversal, BuildTargetGroup.Standalone, "MacUniversal", PlatformIcons[0]));
#endif

            //Resets build state
            Building = false;

            //Adds and removes Platforms based on Unity version
#if !UNITY_5_4_OR_NEWER
            Platforms.Add(new Platform("Web Player", "_Web", "Building Web Player", BuildTarget.WebPlayer, BuildTargetGroup.WebPlayer, "WebPlayer", PlatformIcons[10]));
            OptimisedOrder.Add(BuildTarget.WebPlayer, 15);
            OptimisedOrder.Add(BuildTarget.WebPlayerStreamed, 16);
#endif
#if UNITY_5_5_OR_NEWER
            platforms.RemoveAll(x => x.TargetPlatform == BuildTarget.PS3 || x.TargetPlatform == BuildTarget.XBOX360);
            platforms.Add(new Platform("3DS", "_3DS", "Building 3DS Project", BuildTarget.N3DS, BuildTargetGroup.N3DS, PlatformIcons[20]));
            OptimisedOrder.Add(BuildTarget.N3DS, 25);
#if UNITY_5_6_OR_NEWER
            platforms.AddRange(new Platform[]
            {
                new Platform("Switch", "_Switch", "Building Switch Project", BuildTarget.Switch, BuildTargetGroup.Switch, PlatformIcons[21]),
                new Platform("Facebook Gameroom", "_FB.exe", "Building Facebook Gameroom Executable", BuildTarget.StandaloneWindows, BuildTargetGroup.Facebook, PlatformIcons[22]),
                new Platform("Facebook WebGL", "_FB_WebGL", "Building Facebook WebGL Project", BuildTarget.WebGL, BuildTargetGroup.Facebook, PlatformIcons[22])
            });
            OptimisedOrder.Add(BuildTarget.Switch, 26);
#endif
#endif
            //Removes uninstalled Platforms
            RemovePlatforms();

#pragma warning restore 0618

            dynamicLiterals = new List<DynamicLiteral>()
            {
                new TimeLiteral("#TIME#", null),
                new DateLiteral("#DATE#", null),
                new DynamicLiteral("#VERSION#", () => Application.version),
#if UNITY_2017_1_OR_NEWER
                new DynamicLiteral("#APP-ID#", () => PlayerSettings.applicationIdentifier),
#if !UNITY_2019_3_OR_NEWER
                new DynamicLiteral("#NET-VER#", PlayerSettings.scriptingRuntimeVersion.ToString),
#endif
#endif
                new DynamicLiteral("#PRODUCT-NAME#", () => PlayerSettings.productName),
                new DynamicLiteral("#COMPANY-NAME#", () => PlayerSettings.companyName),
                new DynamicLiteral("#UNITY-VERSION#", () => Application.unityVersion),
                new DynamicLiteral("#LICENSE#", () => Application.HasProLicense() ? "Pro" : "Personal")
            };

            //Loads data
            LoadData();

            GetActivePlatform();
        }

        /// <summary>Removes any Platforms that the user does not have installed.</summary>
        private void RemovePlatforms()
        {
            for (int i = platforms.Count - 1; i >= 0; i--)
            {
                //Removes unsupported platforms
                if (!BackendCompatibility.GetPlatformIsSupported(platforms[i].TargetPlatform, platforms[i].TargetGroup)) { platforms.RemoveAt(i); }
            }
        }

        //Saves data
        private void OnDestroy() { if (AutoSaveDestroy) { SaveData(); } }
        private void OnLostFocus() { if (AutoSaveLoseFocus) { SaveData(); } }
#endregion

#region SaveDataManagement
        /// <summary>Loads the Build Automator configuration data.</summary>
        /// <param name="Prefix">Prefix to add to all saved data.</param>
        /// <param name="LoadPresets">If preset data should be loaded.</param>
        private void LoadData(string Prefix = "", bool LoadPresets = true)
        {
            //Loads from disk
            DataStore.LoadFromDisk();

            //Loads data
            SaveOnBuild = DataStore.Get(Prefix + "SaveOnBuild", false);
            BuildName = DataStore.Get(Prefix + "BuildName", PlayerSettings.productName);
            OutputPath = DataStore.Get(Prefix + "OutputPath", "");
            Batch = DataStore.Get(Prefix + "Batch", false);
            UniBatch = DataStore.Get(Prefix + "UniBatch", false);
            UniversalBatchPath = DataStore.Get(Prefix + "UniversalBatchPath", "");
            UniDefine = DataStore.Get(Prefix + "UniDefine", "");
            Revert = DataStore.Get(Prefix + "Revert", false);
            Dev = DataStore.Get(Prefix + "Dev", false);
            Subfolders = DataStore.Get(Prefix + "Subfolders", false);
            DevDefine = DataStore.Get(Prefix + "DevDefine", "");
            DesiredPathType = DataStore.Get(Prefix + "RelativePaths", true) ? PathType.Relative : PathType.Absolute;

            //Scene data
            sceneList.Resize(DataStore.Get(Prefix + "SceneCount", 0));
            sceneList.useEditorScenes = DataStore.Get(Prefix + "UseEditorScenes", false);
            for (int i = 0; i < sceneList.Count; i++) { sceneList[i] = AssetDatabase.LoadAssetAtPath<SceneAsset>(DataStore.Get(Prefix + "Scene" + i.ToString(), "")); }
            if (!sceneList.useEditorScenes && !sceneList.ValidSceneList) { sceneList[0] = AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorSceneManager.GetActiveScene().path); }

            //Platform data
            foreach (Platform Target in platforms)
            {
                Target.Enabled = DataStore.Get(Prefix + Target.Name, false);
                Target.BatchEnabled = DataStore.Get(Prefix + Target.Name + "Batch", false);
                Target.BatchPath = DataStore.Get(Prefix + Target.Name + "BatchPath", "");
                Target.ScriptingDefine = DataStore.Get(Prefix + Target.Name + "Define", "");
                Target.CurrentBackend = DataStore.Get(Prefix + Target.Name + "CurrentBackend", 0);
#if UNITY_2018_1_OR_NEWER
                Target.CppCompilerConfig = (Il2CppCompilerConfiguration)DataStore.Get(Prefix + Target.Name + "C++Config", 0);
#endif
                Target.NamePrefix = DataStore.Get(Prefix + Target.Name + "NamePrefix", "");
                Target.NameSuffix = DataStore.Get(Prefix + Target.Name + "NameSuffix", "");
            }

            for (int i = 0; i < dynamicLiterals.Count; i++) { dynamicLiterals[i].SetSaveData(DataStore.Get(Prefix + dynamicLiterals[i].identifier, "")); }

            //Foldout data
            LiteralsFoldout = DataStore.Get(Prefix + "LiteralsFoldout", true);
            InformationFoldout = DataStore.Get(Prefix + "InformationFoldout", true);
            ScenesFoldout = DataStore.Get(Prefix + "ScenesFoldout", true);
            PlatformsFoldout = DataStore.Get(Prefix + "PlatformsFoldout", true);
            SettingsFoldout = DataStore.Get(Prefix + "SettingsFoldout", true);
            PreprocessorsFoldout = DataStore.Get(Prefix + "PreprocessorsFoldout", true);
            BatchFoldout = DataStore.Get(Prefix + "BatchFoldout", true);
            OtherFoldout = DataStore.Get(Prefix + "OtherFoldout", true);

#if UNITY_2018_1_OR_NEWER
            LoggingFoldout = DataStore.Get(Prefix + "LoggingFoldout", true);
            DumpToLog = DataStore.Get(Prefix + "DumpToLog", false);
            LogFilePath = DataStore.Get(Prefix + "LogFilePath", "");
#endif

            //Presets
            if (LoadPresets)
            {
                //Save settings
                AutoSaveFoldout = DataStore.Get(Prefix + "AutoSaveFoldout", true);
                AutoSaveDestroy = DataStore.Get(Prefix + "AutoSaveDestroy", true);
                AutoSaveLoseFocus = DataStore.Get(Prefix + "AutoSaveLoseFocus", false);

                PresetsFoldout = DataStore.Get(Prefix + "PresetsFoldout", true);
                PresetPipelineFoldout = DataStore.Get(Prefix + "PresetPipelineFoldout", true);

                //Loads data
                CurrentPresetID = DataStore.Get(Prefix + "CurrentPresetID", 0);
                AvailablePresets = DataStore.Get(Prefix + "AvailablePresets", "None").Split('%').Distinct().ToArray();

                //Corrects preset choices if "None" is not available
                if (AvailablePresets.Length == 0) { AvailablePresets = new string[] { "None" }; }
                else if (AvailablePresets[0] != "None")
                {
                    string[] Presets = AvailablePresets;
                    AvailablePresets = new string[Presets.Length + 1];
                    AvailablePresets[0] = "None";
                    for (int i = 0; i < Presets.Length; i++) { AvailablePresets[i + 1] = Presets[i]; }
                }

                //Pipeline
                RevertPipeline = DataStore.Get(Prefix + "RevertPipeline", false);
                presetPipelineList.Resize(DataStore.Get(Prefix + "PresetPipelineCount", 0));
                for (int i = 0; i < presetPipelineList.Count; i++) { presetPipelineList[i] = DataStore.Get(Prefix + "PresetPipeline" + i.ToString(), ""); }
            }

            //Non BA settings
            //Global
            EditorUserBuildSettings.connectProfiler = DataStore.Get(Prefix + "EditorUserBuildSettings.connectProfiler", EditorUserBuildSettings.connectProfiler);
            EditorUserBuildSettings.allowDebugging = DataStore.Get(Prefix + "EditorUserBuildSettings.allowDebugging", EditorUserBuildSettings.allowDebugging);
#if UNITY_2017_1_OR_NEWER
            EditorUserBuildSettings.buildScriptsOnly = DataStore.Get(Prefix + "EditorUserBuildSettings.buildScriptsOnly", EditorUserBuildSettings.buildScriptsOnly);
            CompressionType = (CompressionOptions)DataStore.Get(Prefix + "CompressionType", (int)CompressionType);
#endif
            //Linux
            EditorUserBuildSettings.enableHeadlessMode = DataStore.Get(Prefix + "EditorUserBuildSettings.enableHeadlessMode", EditorUserBuildSettings.enableHeadlessMode);
            //iOS/tvOS
#if UNITY_5_5_OR_NEWER
            EditorUserBuildSettings.iOSBuildConfigType = (iOSBuildType)DataStore.Get(Prefix + "EditorUserBuildSettings.iOSBuildConfigType", (int)EditorUserBuildSettings.iOSBuildConfigType);
#endif
            //Android
            PlayerSettings.Android.targetArchitectures = (AndroidArchitecture)DataStore.Get(Prefix + "PlayerSettings.Android.targetArchitectures", (int)PlayerSettings.Android.targetArchitectures);
            EditorUserBuildSettings.androidBuildSubtarget = (MobileTextureSubtarget)DataStore.Get(Prefix + "EditorUserBuildSettings.androidBuildSubtarget", (int)EditorUserBuildSettings.androidBuildSubtarget);
#if UNITY_5_5_OR_NEWER
            EditorUserBuildSettings.androidBuildSystem = (AndroidBuildSystem)DataStore.Get(Prefix + "EditorUserBuildSettings.androidBuildSystem", (int)EditorUserBuildSettings.androidBuildSystem);
            EditorUserBuildSettings.exportAsGoogleAndroidProject = DataStore.Get(Prefix + "EditorUserBuildSettings.exportAsGoogleAndroidProject", EditorUserBuildSettings.exportAsGoogleAndroidProject);
#endif
            //WSA/UWP
#if UNITY_2017_1_OR_NEWER
            EditorUserBuildSettings.wsaSubtarget = (WSASubtarget)DataStore.Get(Prefix + "EditorUserBuildSettings.wsaSubtarget", (int)EditorUserBuildSettings.wsaSubtarget);
#else
            EditorUserBuildSettings.wsaSDK = (WSASDK)DataStore.Get(Prefix + "EditorUserBuildSettings.wsaSDK", (int)EditorUserBuildSettings.wsaSDK);
#endif
            EditorUserBuildSettings.wsaUWPBuildType = (WSAUWPBuildType)DataStore.Get(Prefix + "EditorUserBuildSettings.wsaUWPBuildType", (int)EditorUserBuildSettings.wsaUWPBuildType);

            //WebPlayer
#if !UNITY_5_4_OR_NEWER
            EditorUserBuildSettings.webPlayerStreamed = DataStore.Get(Prefix + "EditorUserBuildSettings.webPlayerStreamed", EditorUserBuildSettings.webPlayerStreamed);
            EditorUserBuildSettings.webPlayerOfflineDeployment = DataStore.Get(Prefix + "EditorUserBuildSettings.webPlayerOfflineDeployment", EditorUserBuildSettings.webPlayerOfflineDeployment);
#endif
            //WebGL
#if !UNITY_5_4_OR_NEWER
            EditorUserBuildSettings.webGLOptimizationLevel = DataStore.Get(Prefix + "EditorUserBuildSettings.webGLOptimizationLevel", (int)EditorUserBuildSettings.webGLOptimizationLevel);
#endif
            //FB

            //Xbox One
            EditorUserBuildSettings.xboxBuildSubtarget = DataStore.Get(Prefix + "EditorUserBuildSettings.xboxBuildSubtarget", default(XboxBuildSubtarget));
            EditorUserBuildSettings.xboxOneDeployMethod = DataStore.Get(Prefix + "EditorUserBuildSettings.xboxOneDeployMethod", default(XboxOneDeployMethod));
            EditorUserBuildSettings.streamingInstallLaunchRange = DataStore.Get(Prefix + "EditorUserBuildSettings.streamingInstallLaunchRange", 0);
#if UNITY_2018_2_OR_NEWER
            EditorUserBuildSettings.xboxOneDeployDrive = DataStore.Get(Prefix + "EditorUserBuildSettings.xboxOneDeployDrive", default(XboxOneDeployDrive));
#endif
        }

        /// <summary>Saves the Build Automator configuration data.</summary>
        /// <param name="Prefix">Prefix to add to all saved data.</param>
        /// <param name="SavePresets">If preset data should be saved.</param>
        private void SaveData(string Prefix = "", bool SavePresets = true)
        {
            //Loads data
            DataStore.LoadFromDisk();

            //Saves data
            DataStore.Set(Prefix + "SaveOnBuild", SaveOnBuild);
            DataStore.Set(Prefix + "BuildName", BuildName);
            DataStore.Set(Prefix + "OutputPath", OutputPath);
            DataStore.Set(Prefix + "Dev", Dev);
            DataStore.Set(Prefix + "Subfolders", Subfolders);
            DataStore.Set(Prefix + "DevDefine", DevDefine);
            DataStore.Set(Prefix + "Batch", Batch);
            DataStore.Set(Prefix + "UniBatch", UniBatch);
            DataStore.Set(Prefix + "UniversalBatchPath", UniversalBatchPath);
            DataStore.Set(Prefix + "UniDefine", UniDefine);
            DataStore.Set(Prefix + "Revert", Revert);
            DataStore.Set(Prefix + "RelativePaths", DesiredPathType == PathType.Relative);

            //Scene data
            DataStore.Set(Prefix + "SceneCount", sceneList.Count);
            DataStore.Set(Prefix + "UseEditorScenes", sceneList.useEditorScenes);
            for (int i = 0; i < sceneList.Count; i++) { DataStore.Set(Prefix + "Scene" + i.ToString(), AssetDatabase.GetAssetPath(sceneList[i])); }

            //Platform data
            foreach (Platform Target in platforms)
            {
                DataStore.Set(Prefix + Target.Name, Target.Enabled);
                DataStore.Set(Prefix + Target.Name + "Batch", Target.BatchEnabled);
                DataStore.Set(Prefix + Target.Name + "BatchPath", Target.BatchPath);
                DataStore.Set(Prefix + Target.Name + "Define", Target.ScriptingDefine);
                DataStore.Set(Prefix + Target.Name + "CurrentBackend", Target.CurrentBackend);
#if UNITY_2018_1_OR_NEWER
                DataStore.Set(Prefix + Target.Name + "C++Config", (int)Target.CppCompilerConfig);
#endif
                DataStore.Set(Prefix + Target.Name + "NamePrefix", Target.NamePrefix);
                DataStore.Set(Prefix + Target.Name + "NameSuffix", Target.NameSuffix);
            }

            for (int i = 0; i < dynamicLiterals.Count; i++) { DataStore.Set(Prefix + dynamicLiterals[i].identifier, dynamicLiterals[i].GetSaveData()); }

            //Foldout
            DataStore.Set(Prefix + "InformationFoldout", InformationFoldout);
            DataStore.Set(Prefix + "LiteralsFoldout", LiteralsFoldout);
            DataStore.Set(Prefix + "ScenesFoldout", ScenesFoldout);
            DataStore.Set(Prefix + "PlatformsFoldout", PlatformsFoldout);
            DataStore.Set(Prefix + "SettingsFoldout", SettingsFoldout);
            DataStore.Set(Prefix + "PreprocessorsFoldout", PreprocessorsFoldout);
            DataStore.Set(Prefix + "BatchFoldout", BatchFoldout);
            DataStore.Set(Prefix + "OtherFoldout", OtherFoldout);

#if UNITY_2018_1_OR_NEWER
            DataStore.Set(Prefix + "LoggingFoldout", LoggingFoldout);
            DataStore.Set(Prefix + "DumpToLog", DumpToLog);
            DataStore.Set(Prefix + "LogFilePath", LogFilePath);
#endif

            //Presets
            if (SavePresets)
            {
                DataStore.Set(Prefix + "AutoSaveFoldout", AutoSaveFoldout);
                DataStore.Set(Prefix + "AutoSaveDestroy", AutoSaveDestroy);
                DataStore.Set(Prefix + "AutoSaveLoseFocus", AutoSaveLoseFocus);

                DataStore.Set(Prefix + "PresetsFoldout", PresetsFoldout);
                DataStore.Set(Prefix + "PresetPipelineFoldout", PresetPipelineFoldout);

                DataStore.Set(Prefix + "CurrentPresetID", CurrentPresetID);
                DataStore.Set(Prefix + "AvailablePresets", String.Join("%", AvailablePresets));

                //Pipeline
                DataStore.Set(Prefix + "RevertPipeline", RevertPipeline);
                DataStore.Set(Prefix + "PresetPipelineCount", presetPipelineList.Count);
                for (int i = 0; i < presetPipelineList.Count; i++) { DataStore.Set(Prefix + "PresetPipeline" + i.ToString(), presetPipelineList[i]); }

            }

            //Non BA settings
            //Global
            DataStore.Set(Prefix + "EditorUserBuildSettings.connectProfiler", EditorUserBuildSettings.connectProfiler);
            DataStore.Set(Prefix + "EditorUserBuildSettings.allowDebugging", EditorUserBuildSettings.allowDebugging);
#if UNITY_2017_1_OR_NEWER
            DataStore.Set(Prefix + "EditorUserBuildSettings.buildScriptsOnly", EditorUserBuildSettings.buildScriptsOnly);
#endif
            //Linux
            DataStore.Set(Prefix + "EditorUserBuildSettings.enableHeadlessMode", EditorUserBuildSettings.enableHeadlessMode);
            //iOS/tvOS
#if UNITY_5_5_OR_NEWER
            DataStore.Set(Prefix + "EditorUserBuildSettings.iOSBuildConfigType", (int)EditorUserBuildSettings.iOSBuildConfigType);
#endif
            //Android
            DataStore.Set(Prefix + "PlayerSettings.Android.targetArchitectures", (int)PlayerSettings.Android.targetArchitectures);
            DataStore.Set(Prefix + "EditorUserBuildSettings.androidBuildSubtarget", (int)EditorUserBuildSettings.androidBuildSubtarget);
#if UNITY_5_5_OR_NEWER
            DataStore.Set(Prefix + "EditorUserBuildSettings.androidBuildSystem", (int)EditorUserBuildSettings.androidBuildSystem);
            DataStore.Set(Prefix + "EditorUserBuildSettings.exportAsGoogleAndroidProject", EditorUserBuildSettings.exportAsGoogleAndroidProject);
#endif
            //WSA/UWP
#if UNITY_2017_1_OR_NEWER
            DataStore.Set(Prefix + "EditorUserBuildSettings.wsaSubtarget", (int)EditorUserBuildSettings.wsaSubtarget);
            DataStore.Set(Prefix + "CompressionType", (int)CompressionType);
#else
            DataStore.Set(Prefix + "EditorUserBuildSettings.wsaSDK", (int)EditorUserBuildSettings.wsaSDK);
#endif
            DataStore.Set(Prefix + "EditorUserBuildSettings.wsaUWPBuildType", (int)EditorUserBuildSettings.wsaUWPBuildType);

            //WebPlayer
#if !UNITY_5_4_OR_NEWER
            DataStore.Set(Prefix + "EditorUserBuildSettings.webPlayerStreamed", EditorUserBuildSettings.webPlayerStreamed);
            DataStore.Set(Prefix + "EditorUserBuildSettings.webPlayerOfflineDeployment", EditorUserBuildSettings.webPlayerOfflineDeployment);
#endif
            //WebGL
#if !UNITY_5_4_OR_NEWER
            DataStore.Set(Prefix + "EditorUserBuildSettings.webGLOptimizationLevel", (int)EditorUserBuildSettings.webGLOptimizationLevel);
#endif
            //FB

            //Xbox One
            DataStore.Set(Prefix + "EditorUserBuildSettings.xboxBuildSubtarget", (int)EditorUserBuildSettings.xboxBuildSubtarget);
            DataStore.Set(Prefix + "EditorUserBuildSettings.xboxOneDeployMethod", (int)EditorUserBuildSettings.xboxOneDeployMethod);
            DataStore.Set(Prefix + "EditorUserBuildSettings.streamingInstallLaunchRange", EditorUserBuildSettings.streamingInstallLaunchRange);
#if UNITY_2018_2_OR_NEWER
            DataStore.Set(Prefix + "EditorUserBuildSettings.xboxOneDeployDrive", (int)EditorUserBuildSettings.xboxOneDeployDrive);
#endif

            //Saves to disk
            DataStore.SaveToDisk();
        }
#endregion

#region PresetManagement
        /// <summary>Creates a new preset.</summary>
        /// <param name="DesiredName">The desired preset of the new preset.</param>
        /// <returns>Any errors encountered during the preset creation.</returns>
        public string CreateNewPreset(string DesiredName)
        {
            //Validates the name for errors
            if (String.IsNullOrEmpty(DesiredName) || String.IsNullOrEmpty(DesiredName.Trim())) { return "A name must be specified for the preset."; }
            if (DesiredName.Contains("%")) { return "Name '" + DesiredName + "' is not a valid name as the % char is banned."; }
            if (AvailablePresets.Contains(DesiredName)) { return "Preset '" + DesiredName + "' already exists."; }

            //Creates the new preset adding it to the end of the array
            string[] OldPresets = AvailablePresets;
            AvailablePresets = new string[OldPresets.Length + 1];
            for (int i = 0; i < OldPresets.Length; i++) { AvailablePresets[i] = OldPresets[i]; }
            AvailablePresets[OldPresets.Length] = DesiredName;

            //No errors
            return "";
        }

        /// <summary>Loads the selected preset.</summary>
        /// <param name="PresetID">The ID of the preset to load.</param>
        private void LoadPreset(int PresetID)
        {
            //Loads
            LoadData(AvailablePresets[PresetID], false);
            SaveData();

            //Notifies
            ShowNotification(new GUIContent("Preset '" + AvailablePresets[PresetID] + "' loaded.", BAIcon));
        }

        /// <summary>Saves the selected preset.</summary>
        /// <param name="PresetID">The ID of the preset to save.</param>
        private void SavePreset(int PresetID)
        {
            //Saves
            SaveData(AvailablePresets[PresetID], false);
            SaveData();

            //Notifies
            ShowNotification(new GUIContent("Preset '" + AvailablePresets[PresetID] + "' saved.", BAIcon));
        }

        /// <summary>Deletes the selected preset.</summary>
        /// <param name="PresetID">The ID of the preset to delete.</param>
        private void DeletePreset(int PresetID)
        {
            //Caches preset name
            string DeletedPreset = AvailablePresets[PresetID];

            //Removes preset
            string[] OldPresets = AvailablePresets;
            AvailablePresets = new string[OldPresets.Length - 1];
            for (int i = 0; i < AvailablePresets.Length; i++)
            {
                if (i < PresetID) { AvailablePresets[i] = OldPresets[i]; }
                else { AvailablePresets[i] = OldPresets[i + 1]; }
            }

            //Resets current preset
            CurrentPresetID = 0;

            //Notifies
            ShowNotification(new GUIContent("Preset '" + DeletedPreset + "' deleted.", BAIcon));
        }
#endregion

#region Utils
        public string FinaliseBAString(string Input)
        {
            for (int i = 0; i < dynamicLiterals.Count; i++) { Input = Input.Replace(dynamicLiterals[i].identifier, dynamicLiterals[i].GenerateLiteral()); }
            return Input;
        }
#endregion

#region PresetPipelineManagement
        /// <summary>Prepares presets for pipeline execution.</summary>
        public void PreparePresets()
        {
            SaveData();
            presetPipelineList.RemoveAll(x => string.IsNullOrEmpty(x));
            presetPipelineList.RemoveAll(x => x == "None");
            presetPipelineList.RemoveAll(x => !AvailablePresets.Contains(x));
        }

        /// <summary>Executes the preset pipeline.</summary>
        public void ExecutePresetPipeline()
        {
            int OriginalPreset = CurrentPresetID;
            for (int i = 0; i < presetPipelineList.Count; i++)
            {
                CurrentPresetID = Array.IndexOf(AvailablePresets, presetPipelineList[i]);
                LoadPreset(CurrentPresetID);
                PrepareScenes();
                PerformBuilds();
            }

            if (OriginalPreset > 0 && RevertPipeline)
            {
                CurrentPresetID = OriginalPreset;
                LoadPreset(CurrentPresetID);
            }
        }
#endregion

#region SceneManagement
        /// <summary>Prepares scenes for build sequence.</summary>
        public string[] PrepareScenes()
        {
            SaveData();

            if (sceneList.useEditorScenes)
            {
#if UNITY_5_6_OR_NEWER
                scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);
#else
                List<EditorBuildSettingsScene> sceneObjects = EditorBuildSettings.scenes.ToList();
                sceneObjects.RemoveAll((EditorBuildSettingsScene x) => !x.enabled);
                scenes = new string[sceneObjects.Count];
                for (int i = 0; i < scenes.Length; i++) { scenes[i] = sceneObjects[i].path; }
#endif
            }
            else
            {
                sceneList.RemoveAll(x => x == null);
                scenes = new string[sceneList.Count];
                for (int i = 0; i < sceneList.Count; i++) { scenes[i] = AssetDatabase.GetAssetPath(sceneList[i]); }
            }

            if (SaveOnBuild)
            {
                SceneAsset currentScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorSceneManager.GetActiveScene().path);
                if (sceneList.Contains(currentScene)) { EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene()); }
            }

            return scenes;
        }
#endregion

#region BuildManagement
        /// <summary>Finds the currently active Platform.</summary>
        /// <returns>The active Platform.</returns>
        private Platform GetActivePlatform()
        {
            //Finds active Platform
            Platform ActivePlatform = platforms.Find(x => x.TargetPlatform == EditorUserBuildSettings.activeBuildTarget && x.TargetGroup == EditorUserBuildSettings.selectedBuildTargetGroup);
            if (ActivePlatform == null) { ActivePlatform = platforms.Find(x => x.TargetPlatform == EditorUserBuildSettings.activeBuildTarget); }

            //Fallback / last resort
            if (ActivePlatform == null)
            {
                if (platforms.Count > 0) { ActivePlatform = platforms[0]; }
                else { ActivePlatform = new Platform("None", "", "", BuildTarget.StandaloneWindows, BuildTargetGroup.Unknown); }
            }

            //Sets the state of all Platforms
            foreach (Platform Target in platforms)
            {
                if (Target == ActivePlatform)
                {
                    Target.IsActive = true;
                    Target.CurrentState = Platform.PlatformState.Active;
                }
                else
                {
                    Target.IsActive = false;
                    Target.CurrentState = Platform.PlatformState.None;
                }
            }

            //Returns active Platform.
            CurrentPlatform = ActivePlatform;
            return ActivePlatform;
        }

        /// <summary>Updates the progress bar with the current details.</summary>
        /// <param name="Job">The current job in progress.</param>
        public void UpdateProgress(string Job)
        {
            //Updates job and progress status
            CurrentJobNumber++;
            CurrentJob = Job;
            if (CurrentJobNumber <= Jobs)
            {
                JobLabel = "Job " + CurrentJobNumber.ToString() + "/" + Jobs.ToString() + ": " + CurrentJob;
                Progress = (CurrentJobNumber - 0.5f) / Jobs;
            }
            else
            {
                JobLabel = "Reverting to Original Platform";
                Progress = 1f;
            }
            EditorUtility.DisplayProgressBar("Builds in progress", JobLabel, Progress);
        }

        /// <summary>Optimises the build order.</summary>
        /// <returns>A list of the same Platforms in an optimised order.</returns>
        /// <param name="PlatformList">Platform list.</param>
        /// <param name="CurrentPlatform">Current Platform.</param>
        private List<Platform> OptimiseBuildOrder(List<Platform> PlatformList, Platform CurrentPlatform)
        {
            //Copies List
            List<Platform> OptimisedList = new List<Platform>(PlatformList);

            //Sorts into order
            List<Platform> UnoptimisedPlatforms = OptimisedList.PopAll(x => !OptimisedOrder.ContainsKey(x.TargetPlatform));
            OptimisedList = OptimisedList.OrderBy(x => x.TargetGroup).ToList();
            OptimisedList = OptimisedList.OrderBy(x => OptimisedOrder[x.TargetPlatform]).ToList();
            OptimisedList.AddRange(UnoptimisedPlatforms);

            //Moves all of same target to beginning
            List<Platform> SameTarget = OptimisedList.PopAll(x => x.TargetPlatform == CurrentPlatform.TargetPlatform);
            OptimisedList.InsertRange(0, SameTarget);

            //Moves this Platform to the beginning
            OptimisedList.Remove(CurrentPlatform);
            OptimisedList.Insert(0, CurrentPlatform);

            return OptimisedList;

        }

        /// <summary>Begins building to each selected Platform.</summary>
        public void PerformBuilds()
        {
            Building = true;

            OldPlatform = CurrentPlatform;

            //Optimises build order
            List<Platform> OptimisedPlatforms = OptimiseBuildOrder(platforms, CurrentPlatform);

            //Sets states
            foreach (Platform Target in OptimisedPlatforms)
            {
                if (Target.Enabled) { Target.CurrentState = Platform.PlatformState.Queued; }
                else { Target.CurrentState = Platform.PlatformState.None; }
            }

            //Calculates total jobs
            Jobs = 0;
            CurrentJobNumber = 0;
            foreach (Platform Target in OptimisedPlatforms) { if (Target.Enabled) { Jobs++; } }
            if (Batch)
            {
                foreach (Platform Target in OptimisedPlatforms) { if (Target.BatchEnabled && Target.Enabled) { Jobs++; } }
                if (UniBatch) { Jobs++; }
            }

#if UNITY_2018_1_OR_NEWER
            if (DumpToLog && !string.IsNullOrEmpty(LogFilePath))
            {
                //Creates log file
                string DesiredPath = FinaliseBAString(LogFilePath);
                if (DesiredPathType == PathType.Relative) { DesiredPath = PathHandling.Combine(RootPath, DesiredPath); }

                try
                {
                    string DirectoryPath = Path.GetDirectoryName(DesiredPath);
                    if (!Directory.Exists(DirectoryPath)) { Directory.CreateDirectory(DirectoryPath); }
                    CurrentLogWriter = new StreamWriter(DesiredPath);
                }
                catch (Exception e)
                {
                    Debug.LogError("Failed to create log file: " + e.Message);
                    CurrentLogWriter = null;
                }
            }
            else { CurrentLogWriter = null; }
#endif

            //Executes builds
            foreach (Platform Target in OptimisedPlatforms)
            {
                if (Target.Enabled)
                {
                    //Executes build
                    if (Target.BuildToPlatform(this))
                    {
                        //Marks others as inactive
                        foreach (Platform OtherTarget in OptimisedPlatforms) { if (OtherTarget != Target) { OtherTarget.IsActive = false; } }
                        CurrentPlatform = Target;
                    }

                    else { if (Target.TargetPlatform == EditorUserBuildSettings.activeBuildTarget && Target.TargetGroup == EditorUserBuildSettings.selectedBuildTargetGroup) { CurrentPlatform = Target; } }
                }
            }

            //Runs universal batch script
            if (Batch && UniBatch)
            {
                UpdateProgress("Running Universal Command Line Scripts");
                try { System.Diagnostics.Process.Start(FinaliseBAString(PathHandling.Combine(BuildPath, UniversalBatchPath))); }
                catch { Debug.LogError("Unable to start universal batch script at " + PathHandling.Combine(BuildPath, UniversalBatchPath) + "."); }
            }

            //Reverts Platform
            if (Revert)
            {
                if (CurrentPlatform != OldPlatform)
                {
                    UpdateProgress("Reverting to Original Platform (" + OldPlatform.Name + ").");

                    if (OldPlatform.SwitchToPlatform(CurrentPlatform))
                    {
                        CurrentPlatform = OldPlatform;
                        CurrentPlatform.UpdateScriptingDefines(this);
                        PlayerSettings.SetScriptingDefineSymbolsForGroup(CurrentPlatform.TargetGroup, CurrentPlatform.ScriptingDefine);
                    }
                    else { Debug.LogError("Unable to revert Platform to " + OldPlatform.Name + "."); }
                }
            }

            //Resets states
            foreach (Platform Target in OptimisedPlatforms)
            {
                if (Target.IsActive) { Target.CurrentState = Platform.PlatformState.Active; }
                else { Target.CurrentState = Platform.PlatformState.None; }
            }

#if UNITY_2018_1_OR_NEWER
            if (CurrentLogWriter != null)
            {
                //Closes file and ends
                CurrentLogWriter.Flush();
                CurrentLogWriter.Close();
                CurrentLogWriter.Dispose();
                CurrentLogWriter = null;
            }
#endif

            Building = false;
            EditorUtility.ClearProgressBar();
        }
#endregion
    }
}

