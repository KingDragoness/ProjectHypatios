using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;

namespace QFSW.BA
{
    public partial class BuildAutomator
    {
        //Textures
        /// <summary>Textures for Platform icons.</summary>
        private List<Texture> PlatformIcons;
        /// <summary>Banner to be displayed on the window.</summary>
        private Texture Banner;
        /// <summary>Icon for file browser buttons.</summary>
        private Texture FolderIcon;
        /// <summary>Build Automator Icon.</summary>
        private Texture BAIcon;

        //Foldouts
        /// <summary>Expands the BA Presets sub-menu.</summary>
        private bool AutoSaveFoldout = true;
        /// <summary>Expands the BA Presets sub-menu.</summary>
        private bool PresetsFoldout = true;
        /// <summary>Expands the BA Preset pipeline sub-menu.</summary>
        private bool PresetPipelineFoldout = true;
        /// <summary>Expands the Build Information sub-menu.</summary>
        private bool InformationFoldout = true;
        private bool LiteralsFoldout = true;
        /// <summary>Expands the Scenes sub-menu.</summary>
        private bool ScenesFoldout = true;
        /// <summary>Expands the Build Targets sub-menu.</summary>
        private bool PlatformsFoldout = true;
        /// <summary>Expands the Build Settings sub-menu.</summary>
        private bool SettingsFoldout = true;
        /// <summary>Expands the Scripting Define Symbols sub-menu.</summary>
        private bool PreprocessorsFoldout = true;
        /// <summary>Expands the Batch Scripts sub-menu.</summary>
        private bool BatchFoldout = true;
        /// <summary>Expands the Other Settings sub-menu.</summary>
        private bool OtherFoldout = true;
#if UNITY_2018_1_OR_NEWER
        /// <summary>Expands the Logging sub-menu.</summary>
        private bool LoggingFoldout = true;
#endif

        //Custom GUIStyles
        /// <summary>GUIStyle for file picker buttons.</summary>
        private GUIStyle FolderStyle;
        /// <summary>The GUIStyle for the foldouts.</summary>
        private GUIStyle BoldFoldout;
        /// <summary>The GUIStyles for the different PlatformStates</summary>
        private GUIStyle[] PlatformStateStyles;
        private GUIStyle RichLabel;

        /// <summary>Scrollbar position.</summary>
        Vector2 ScrollPos = new Vector2();

        //Menu-bar entry
        [MenuItem("Window/Build Automator #&b")]

        //Shows window
        public static void ShowWindow() { EditorWindow.GetWindow(typeof(BuildAutomator), false, "BA", true); }

        /// <summary>Creates all the custom GUIStyles necessary.</summary>
        private void CreateStyles()
        {
            //Folder Icons
            FolderStyle = new GUIStyle(EditorStyles.label);
            FolderStyle.padding = new RectOffset(-1, -1, -1, -1);
            FolderStyle.fixedHeight = 14;
            FolderStyle.fixedWidth = 14;

            //Foldouts
            BoldFoldout = new GUIStyle(EditorStyles.foldout);
            BoldFoldout.fontStyle = FontStyle.Bold;

            RichLabel = new GUIStyle(EditorStyles.label);
            RichLabel.richText = true;

            //PlatformStates
            PlatformStateStyles = new GUIStyle[5];
            for (int i = 0; i < PlatformStateStyles.Length; i++)
            {
                PlatformStateStyles[i] = new GUIStyle();
                PlatformStateStyles[i].normal.background = new Texture2D(1, 1);
                PlatformStateStyles[i].stretchHeight = true;
                PlatformStateStyles[i].stretchWidth = true;
                PlatformStateStyles[i].fixedHeight = 20;
            }

            PlatformStateStyles[(int)Platform.PlatformState.None].normal.background.SetPixel(0, 0, new Color(0, 0, 0, 0));
            PlatformStateStyles[(int)Platform.PlatformState.Active].normal.background.SetPixel(0, 0, new Color(0.3f, 0.6f, 1f, 0.7f));
            PlatformStateStyles[(int)Platform.PlatformState.Queued].normal.background.SetPixel(0, 0, new Color(0.9f, 0.7f, 0.3f, 0.7f));
            PlatformStateStyles[(int)Platform.PlatformState.Unsuccessful].normal.background.SetPixel(0, 0, new Color(1f, 0.3f, 0.2f, 0.7f));
            PlatformStateStyles[(int)Platform.PlatformState.Successful].normal.background.SetPixel(0, 0, new Color(0.3f, 0.7f, 0.35f, 0.7f));

            for (int i = 0; i < PlatformStateStyles.Length; i++) { PlatformStateStyles[i].normal.background.Apply(); }
        }

        //Main GUI
        private void OnGUI()
        {
            //Creates Styles
            CreateStyles();

            //Logo display
            GUILayout.Label(Banner, GUILayout.Height(Screen.width * 9 / 80));

            //Error handling
            if (platforms.Count == 0)
            {
                EditorGUILayout.LabelField("This installation of Unity does not have any build targets installed that Build Automator supports. " +
                                            "Please install one or more supported build targets to use Build Automator.", EditorStyles.wordWrappedLabel);
            }
            else
            {
                //Starts scrollable area
                ScrollPos = EditorGUILayout.BeginScrollView(ScrollPos);

                //Displays all settings
                DisplayAutoSave();
                DisplayBAPresets();
                DisplayBAPresetPipeline();
                DisplayLiteralSettings();
                DisplayMainSettings();
                DisplayLoggingSettings();
                DisplayScenesSettings();
                DisplayPlatformSettings();
                DisplayBuildSettings();
                DisplayPreprocessorsSettings();
                DisplayBatchSettings();
                DisplayOtherSettings();

                //Ends Scrollable Area
                EditorGUILayout.EndScrollView();

                //Build and save buttons
                if (!Building)
                {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button(new GUIContent("Save Settings", "Save Build Automator settings to the project.")))
                    {
                        SaveData();
                        ShowNotification(new GUIContent("Project Settings Saved", BAIcon));
                    }
                    if (DisplayBuildButton())
                    {
                        //Starts the build process
                        RemoveNotification();
                        PrepareScenes();
                        PerformBuilds();
                    }
                    EditorGUILayout.EndHorizontal();
                }

                //Updates progress
                if (Building) { EditorGUI.ProgressBar(new Rect(new Vector2(0, Screen.height - 38), new Vector2(Screen.width - 2, 18)), Progress, JobLabel); }

                GUI.enabled = true;
            }
        }

        private void DisplayAutoSave()
        {
            //Autosave settings
            AutoSaveFoldout = EditorGUILayout.Foldout(AutoSaveFoldout, new GUIContent("Auto Save", "Determines the nature of Build Automator's auto saving"), BoldFoldout);
            if (AutoSaveFoldout)
            {
                AutoSaveDestroy = EditorGUILayout.Toggle(new GUIContent("Auto Save on Destroy", "If Build Automator should auto save when the window is destroyed."), AutoSaveDestroy);
                AutoSaveLoseFocus = EditorGUILayout.Toggle(new GUIContent("Auto Save on Focus Loss", "If Build Automator should auto save when the window has lost focus."), AutoSaveLoseFocus);
                EditorGUILayout.Space();
            }
        }

        /// <summary>Displays the BA Presets sub-menu to the window.</summary>
        private void DisplayBAPresets()
        {
            //Preset settings
            PresetsFoldout = EditorGUILayout.Foldout(PresetsFoldout, new GUIContent("Presets", "Allows you to create and use Build Automator Presets."), BoldFoldout);
            if (PresetsFoldout)
            {
                //Displays current preset
                EditorGUILayout.BeginHorizontal();
                CurrentPresetID = EditorGUILayout.Popup("Current Preset", CurrentPresetID, AvailablePresets, GUILayout.Height(16));
                if (GUILayout.Button(new GUIContent("New Preset", "Creates and saves new user defined preset."), EditorStyles.miniButton, GUILayout.Height(16), GUILayout.Width(80))) { NewPresetPopup(); }
                EditorGUILayout.EndHorizontal();

                //Options for loading, saving and deleting presets
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Total Presets: " + (AvailablePresets.Length - 1).ToString(), "There are currently " + (AvailablePresets.Length - 1).ToString() + " created for this project"), EditorStyles.miniLabel, GUILayout.Height(14));
                GUI.enabled = CurrentPresetID > 0;
                if (GUILayout.Button(new GUIContent("Load Preset", "Creates and saves new user defined preset."), EditorStyles.miniButton, GUILayout.Height(16), GUILayout.Width(80))) { LoadPreset(CurrentPresetID); }
                if (GUILayout.Button(new GUIContent("Save Preset", "Loads the configuration from the current preset."), EditorStyles.miniButton, GUILayout.Height(16), GUILayout.Width(80))) { SavePreset(CurrentPresetID); }
                if (GUILayout.Button(new GUIContent("Delete Preset", "Deletes the current preset."), EditorStyles.miniButton, GUILayout.Height(16), GUILayout.Width(80))) { DeletePreset(CurrentPresetID); }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
            }

            GUI.enabled = true;
        }

        /// <summary>Displays a popup window for creating new presets.</summary>
        private void NewPresetPopup() { PopupWindow.Show(new Rect(5, 5, 0, 0), new PresetPopup(this)); }

        /// <summary>Displays the BA Preset pipeline sub-menu to the window.</summary>
        private void DisplayBAPresetPipeline()
        {
            //Pipeline settings
            PresetPipelineFoldout = EditorGUILayout.Foldout(PresetPipelineFoldout, new GUIContent("Preset Pipelining", "Allows you to execute several different presets in a single automation cycle."), BoldFoldout);
            if (PresetPipelineFoldout)
            {
                //Revert
                RevertPipeline = EditorGUILayout.Toggle(new GUIContent("Revert Preset", "Switches back to the original Preset (" + AvailablePresets[CurrentPresetID] + ") after completing the pipeline execution."), RevertPipeline);
                presetPipelineList.UpdateData(AvailablePresets, CurrentPresetID);
                presetPipelineList.DrawList();

                //Execute button
                EditorGUILayout.Space();
                GUI.enabled = !Building;
                if (DisplayPresetBuildButton())
                {
                    //Starts the build process
                    RemoveNotification();
                    PreparePresets();
                    if (presetPipelineList.Count > 0) { ExecutePresetPipeline(); }
                }

                EditorGUILayout.Space();
            }

            GUI.enabled = true;
        }

        /// <summary>Displays the Logging sub menu.</summary>
        private void DisplayLoggingSettings()
        {
#if UNITY_2018_1_OR_NEWER
            LoggingFoldout = EditorGUILayout.Foldout(LoggingFoldout, new GUIContent("Logging", "Settings for post build log file dumping."), BoldFoldout);
            if (LoggingFoldout)
            {
                //Logging toggle
                DumpToLog = EditorGUILayout.Toggle(new GUIContent("Dump to Log File", "If BA should dump build statistics to a log file."), DumpToLog);
                GUI.enabled = DumpToLog;

                //Log file path selector
                GUILayout.BeginHorizontal();
                LogFilePath = EditorGUILayout.TextField(new GUIContent("Log File Path", "The name and path to the log file to which Build Automator will output the log dump."), LogFilePath);

                if (GUILayout.Button(new GUIContent(FolderIcon, "File browser for selecting the desired output name and path."), FolderStyle))
                {
                    string Selection = EditorUtility.OpenFilePanel("Log File", RootPath, "log");
                    if (!String.IsNullOrEmpty(Selection))
                    {
                        if (DesiredPathType == PathType.Absolute) { LogFilePath = Selection.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar); }
                        else
                        {
                            try { LogFilePath = PathHandling.MakeRelative(RootPath, Selection); }
                            catch { Debug.LogError("Could not convert " + Selection.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar) + " to be relative to " + RootPath + ". Have you considered using Absolute path mode instead?"); }
                        }
                    }
                }
                GUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }
            GUI.enabled = true;
#endif
        }

        private void DisplayLiteralSettings()
        {
            LiteralsFoldout = EditorGUILayout.Foldout(LiteralsFoldout, new GUIContent("Dynamic Literals", "All dynamic literals that can be used within Build Automator. When dynamic literals are used, they are replaced at build time."), BoldFoldout);
            if (LiteralsFoldout)
            {
                for (int i = 0; i < dynamicLiterals.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    string LiteralString = "<b>" + dynamicLiterals[i].identifier + " → </b>" + dynamicLiterals[i].GenerateLiteral();
                    string LiteralComment = "Using the literal " + dynamicLiterals[i].identifier + " in any BA strings (e.g. build name, path etc.) will be replaced at buildtime.\nCurrent value: " + dynamicLiterals[i].GenerateLiteral();
                    EditorGUILayout.LabelField(new GUIContent(LiteralString, LiteralComment), RichLabel);
                    dynamicLiterals[i].ShowCustomSettings();
                    GUILayout.EndHorizontal();
                }

                EditorGUILayout.Space();
            }

            GUI.enabled = true;
        }

        /// <summary>Displays the Build Information sub-menu to the window.</summary>
        private void DisplayMainSettings()
        {
            //Main project settings
            InformationFoldout = EditorGUILayout.Foldout(InformationFoldout, new GUIContent("Build Information", "The main settings for your project."), BoldFoldout);
            if (InformationFoldout)
            {
                //Displays current Platform and root directory
                EditorGUILayout.SelectableLabel("Root Directory: " + RootPath + "\nCurrent Platform: " + CurrentPlatform.Name, EditorStyles.miniLabel, GUILayout.Height(28));

                //Creates BuildPath
                DesiredPathType = (PathType)EditorGUILayout.EnumPopup(new GUIContent("Path Type", "Whether to use absolute paths or paths relative to Root Directory Path. Relative paths will be relative to " + RootPath), DesiredPathType);
                if (DesiredPathType == PathType.Relative) { BuildPath = RootPath; }
                else { BuildPath = ""; }

                //BuildName TextField
                BuildName = EditorGUILayout.TextField(new GUIContent("Build Name", "The base name that will be used for all the outputted executables/directories (Defaults to the product name)."), BuildName);

                //OutputPath directory selector
                GUILayout.BeginHorizontal();
                OutputPath = EditorGUILayout.TextField(new GUIContent("Output Path", "The path to the directory to which Build Automator will output the builds; leaving blank will result in the root directory being used.\nCurrent output path: " + FinaliseBAString((String.IsNullOrEmpty(OutputPath) ? RootPath : PathHandling.Combine(BuildPath, OutputPath)))), OutputPath);

                if (GUILayout.Button(new GUIContent(FolderIcon, "File browser for selecting the desired output path."), FolderStyle))
                {
                    string Selection = EditorUtility.OpenFolderPanel("Output directory", RootPath, "");
                    if (!String.IsNullOrEmpty(Selection))
                    {
                        if (DesiredPathType == PathType.Absolute) { OutputPath = Selection.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar); }
                        else
                        {
                            try { OutputPath = PathHandling.MakeRelative(RootPath, Selection); }
                            catch { Debug.LogError("Could not convert " + Selection.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar) + " to be relative to " + RootPath + ". Have you considered using Absolute path mode instead?"); }
                        }
                    }
                }

                GUILayout.EndHorizontal();

                //SubFolders toggle
                Subfolders = EditorGUILayout.Toggle(new GUIContent("Subfolder per Platform", "Gives each platform its own subfolder within the output folder."), Subfolders);

                EditorGUILayout.Space();
            }

            GUI.enabled = true;
        }

        /// <summary>Displays the scene selection system.</summary>
        private void DisplayScenesSettings()
        {
            ScenesFoldout = EditorGUILayout.Foldout(ScenesFoldout, new GUIContent("Scenes", "Which Scenes to use in the build."), BoldFoldout);
            if (ScenesFoldout)
            {
                sceneList.DrawList();
                EditorGUILayout.Space();
            }
            GUI.enabled = true;
        }

        /// <summary>Displays each Platform and allows the user to select which ones to enable.</summary>
        private void DisplayPlatformSettings()
        {
            const float SwitchWidth = 44;
            const float BackendWidth = 60;
            const float TextPadding = 75f;
            const float TextPaddingReversed = 10f;

#if UNITY_2018_1_OR_NEWER
            const float CPPConfigWidth = 64;
#else
            const float CPPConfigWidth = 0;
#endif

            //Platform selection
            GUILayout.BeginHorizontal(PlatformStateStyles[(int)Platform.PlatformState.None]);
            float TextFieldWidth = Mathf.Max((EditorGUIUtility.currentViewWidth - (SwitchWidth + CPPConfigWidth + BackendWidth + EditorGUIUtility.labelWidth + TextPadding)) / 2, 75);
            PlatformsFoldout = EditorGUILayout.Foldout(PlatformsFoldout, new GUIContent("Build Targets", "Which platforms to build to."), BoldFoldout);
            if (PlatformsFoldout)
            {
                //Headers
                GUIStyle HeaderStyle = new GUIStyle(EditorStyles.label);
                HeaderStyle.alignment = TextAnchor.MiddleCenter;
                GUILayout.Label(new GUIContent("Prefix", "Prefix to prepend to the build name."), HeaderStyle, GUILayout.Width(TextFieldWidth));
                GUILayout.Label(new GUIContent("Suffix", "Suffix to append to the build name."), HeaderStyle, GUILayout.Width(TextFieldWidth));
                GUILayout.Space(SwitchWidth + CPPConfigWidth + BackendWidth + TextPaddingReversed + 12.25f);
                GUILayout.EndHorizontal();

                foreach (Platform Target in platforms)
                {
                    GUILayout.BeginHorizontal(PlatformStateStyles[(int)Target.CurrentState]);

                    //Displays Platform and its current build path
                    Target.UpdateBuildPath(this);
                    Target.Enabled = EditorGUILayout.Toggle(new GUIContent(Target.Name, Target.Icon, Target.Name + "\nCurrent path to build: " + FinaliseBAString(Target.BuildPath)), Target.Enabled);

                    //Prefix and suffix options
                    GUI.enabled = !Building;
                    Target.NamePrefix = EditorGUILayout.TextField(new GUIContent("", "Prefix to prepend to the build name when building to " + Target.Name + "."), Target.NamePrefix, GUILayout.Width(TextFieldWidth));
                    Target.NameSuffix = EditorGUILayout.TextField(new GUIContent("", "Suffix to prepend to the build name when building to " + Target.Name + "."), Target.NameSuffix, GUILayout.Width(TextFieldWidth));
                    GUILayout.Space(TextPaddingReversed);

#if UNITY_2018_1_OR_NEWER
                    //IL2CPP C++ compiler config
                    if (Target.SupportedBackends.Contains(ScriptingImplementation.IL2CPP))
                    {
                        GUI.enabled = !Building && Target.CurrentScriptingBackend == ScriptingImplementation.IL2CPP;
                        Target.CppCompilerConfig =  (Il2CppCompilerConfiguration)EditorGUILayout.EnumPopup(Target.CppCompilerConfig, GUILayout.Width(CPPConfigWidth - 4));
                    }
                    else { GUILayout.Space(CPPConfigWidth); }
#endif

                    //Backend switch button
                    GUI.enabled = !(Building || Target.SupportedBackends.Length == 1);
                    Target.CurrentBackend = EditorGUILayout.Popup(Target.CurrentBackend, Target.BackendNames, GUILayout.Width(BackendWidth));

                    //Platform switch button
                    GUI.enabled = !(Building || Target.IsActive);
                    if (GUILayout.Button(new GUIContent("Switch", "Switch build target to " + Target.Name), EditorStyles.miniButton, GUILayout.Height(16), GUILayout.Width(SwitchWidth)))
                    {
                        //Switches Platform
                        SaveData();
                        if (Target.SwitchToPlatform(CurrentPlatform)) { CurrentPlatform = Target; }
                    }
                    GUI.enabled = true;

                    GUILayout.EndHorizontal();
                }

                EditorGUILayout.Space();
            }
            else { GUILayout.EndHorizontal(); }

            GUI.enabled = true;
        }

        /// <summary>Displays all exposed build settings.</summary>
        private void DisplayBuildSettings()
        {
            //Build settings
            SettingsFoldout = EditorGUILayout.Foldout(SettingsFoldout, new GUIContent("Build Settings", "Various settings related to the different build platforms."), BoldFoldout);
            if (SettingsFoldout)
            {
                //Displays settings
                if (platforms.Count > 0)
                {
                    DisplaySharedBuildSettings();
                    DisplayLinuxSettings();
                    DisplayXcodeSettings();
                    DisplayAndroidSettings();
                    DisplayWSASettings();
                    DisplayWebGLSettings();
#if !UNITY_5_4_OR_NEWER
                    DisplayWebPlayerSettings();
#endif
#if UNITY_5_6_OR_NEWER
                    DisplayFBSettings();
#endif
                    DisplayXboxOneSettings();
                }


                EditorGUILayout.Space();
            }

            GUI.enabled = true;
        }

        /// <summary>Displays build settings shared between many Platforms.</summary>
        void DisplaySharedBuildSettings()
        {
            //All Platforms
            GUI.enabled = platforms.Exists(x => x.Enabled);
            Dev = EditorGUILayout.Toggle(new GUIContent("Development Builds", "Enables development builds."), Dev);
            EditorUserBuildSettings.development = Dev;
            EditorUserBuildSettings.connectProfiler = EditorGUILayout.Toggle(new GUIContent("Autoconnect Profiler", "Start the player with a connection to the profiler."), EditorUserBuildSettings.connectProfiler);
#if UNITY_2017_1_OR_NEWER
            CompressionType = (CompressionOptions)EditorGUILayout.EnumPopup(new GUIContent("Build Compression", "What compression type to use for builds"), CompressionType);
#endif
            //WSA / Standalone / iOS / Android / tvOS / Tizen / SamsungTV
            GUI.enabled = platforms.Exists(x => x.Enabled && (x.TargetPlatform == BuildTarget.WSAPlayer || x.TargetGroup == BuildTargetGroup.Standalone
                                                                                                        || x.TargetPlatform == BuildTarget.iOS
                                                                                                        || x.TargetPlatform == BuildTarget.Android
                                                                                                        || x.TargetPlatform == BuildTarget.tvOS
#if !UNITY_2017_3_OR_NEWER
                                                                                                        || x.TargetPlatform == BuildTarget.Tizen
                                                                                                        || x.TargetPlatform == BuildTarget.SamsungTV
#endif
#if UNITY_5_6_OR_NEWER && !UNITY_2019_3_OR_NEWER
                                                                                                        || x.TargetGroup == BuildTargetGroup.Facebook && x.TargetPlatform == BuildTarget.StandaloneWindows
#endif
                                                                                                        ));
            EditorUserBuildSettings.allowDebugging = EditorGUILayout.Toggle(new GUIContent("Script Debugging", "Enables source-level debuggers to connect."), EditorUserBuildSettings.allowDebugging);
#if UNITY_2017_1_OR_NEWER
            GUI.enabled = Dev;
            EditorUserBuildSettings.buildScriptsOnly = EditorGUILayout.Toggle(new GUIContent("Build Scripts Only", "Only rebuilds scripts."), EditorUserBuildSettings.buildScriptsOnly);
#endif
            GUI.enabled = true;
        }

        /// <summary>Displays build settings for Linux.</summary>
        private void DisplayLinuxSettings()
        {
            //Linux
            if (platforms.Exists(x => x.TargetPlatform == BuildTarget.StandaloneLinux64
#if !UNITY_2019_2_OR_NEWER
                                      || x.TargetPlatform == BuildTarget.StandaloneLinux
                                      || x.TargetPlatform == BuildTarget.StandaloneLinuxUniversal
#endif
                                      ))
            {
                GUI.enabled = platforms.Exists(x => x.Enabled && (x.TargetPlatform == BuildTarget.StandaloneLinux64
#if !UNITY_2019_2_OR_NEWER
                                                                  || x.TargetPlatform == BuildTarget.StandaloneLinux
                                                                  || x.TargetPlatform == BuildTarget.StandaloneLinuxUniversal
#endif
                                                                  ));

                EditorUserBuildSettings.enableHeadlessMode = EditorGUILayout.Toggle(new GUIContent("Enable Headless Mode", "Enables a Linux headless build."), EditorUserBuildSettings.enableHeadlessMode);

                GUI.enabled = true;
            }
        }

        /// <summary>Displays build settings for iOS/tvOS.</summary>
        private void DisplayXcodeSettings()
        {
            //Android
            if (platforms.Exists(x => x.TargetPlatform == BuildTarget.iOS || x.TargetPlatform == BuildTarget.tvOS))
            {
                GUI.enabled = platforms.Exists(x => x.Enabled && (x.TargetPlatform == BuildTarget.iOS || x.TargetPlatform == BuildTarget.tvOS));
#if UNITY_5_5_OR_NEWER
                EditorUserBuildSettings.iOSBuildConfigType = (iOSBuildType)EditorGUILayout.EnumPopup(new GUIContent("Xcode Scheme", "Scheme with which the project will be run in Xcode."), EditorUserBuildSettings.iOSBuildConfigType);
#endif
                GUI.enabled = true;
            }
        }

        /// <summary>Displays build settings for Android.</summary>
        private void DisplayAndroidSettings()
        {
            //Android
            if (platforms.Exists(x => x.TargetPlatform == BuildTarget.Android))
            {
                GUI.enabled = platforms.Exists(x => x.Enabled && x.TargetPlatform == BuildTarget.Android);

                PlayerSettings.Android.targetArchitectures = (AndroidArchitecture)EditorGUILayout.EnumFlagsField(new GUIContent("Target Architectures", "A set of CPU architectures for the Android build target."), PlayerSettings.Android.targetArchitectures);
                EditorUserBuildSettings.androidBuildSubtarget = (MobileTextureSubtarget)EditorGUILayout.EnumPopup(new GUIContent("Texture Compression", "Android platform options."), EditorUserBuildSettings.androidBuildSubtarget);
                EditorUserBuildSettings.androidBuildSystem = (AndroidBuildSystem)EditorGUILayout.EnumPopup(new GUIContent("Build System", "Set which build system to use for building the Android package."), EditorUserBuildSettings.androidBuildSystem);
                GUI.enabled = EditorUserBuildSettings.androidBuildSystem == AndroidBuildSystem.Gradle;
                EditorUserBuildSettings.exportAsGoogleAndroidProject = EditorGUILayout.Toggle(new GUIContent("Export Project", "Export Android Project for use with Android StudioGradle or EclipseADT."), EditorUserBuildSettings.exportAsGoogleAndroidProject);

                GUI.enabled = true;
            }
        }

        /// <summary>Displays build settings for WSA/UWP.</summary>
        private void DisplayWSASettings()
        {
            //WSA
            if (platforms.Exists(x => x.TargetGroup == BuildTargetGroup.WSA))
            {
                GUI.enabled = platforms.Exists(x => x.Enabled && x.TargetGroup == BuildTargetGroup.WSA);
#if UNITY_2017_1_OR_NEWER
                EditorUserBuildSettings.wsaSubtarget = (WSASubtarget)EditorGUILayout.EnumPopup(new GUIContent("Target Device", "Specific device type for which resources get optimized."), EditorUserBuildSettings.wsaSubtarget);
#else
                EditorUserBuildSettings.wsaSDK = (WSASDK)EditorGUILayout.EnumPopup(new GUIContent("Windows Store SDK", "Target Windows SDK."), EditorUserBuildSettings.wsaSDK);
                GUI.enabled = EditorUserBuildSettings.wsaSDK == WSASDK.UWP && Platforms.Exists(x => x.Enabled && x.TargetGroup == BuildTargetGroup.WSA);
#endif
                EditorUserBuildSettings.wsaUWPBuildType = (WSAUWPBuildType)EditorGUILayout.EnumPopup(new GUIContent("UWP Build Type", "The build type for Universal Windows Programs."), EditorUserBuildSettings.wsaUWPBuildType);
                GUI.enabled = true;
            }
        }
#if !UNITY_5_4_OR_NEWER
        /// <summary>Displays build settings for Web Player.</summary>
        private void DisplayWebPlayerSettings()
        {
            //WebPlayer
            if (Platforms.Exists(x => x.TargetPlatform == BuildTarget.WebPlayer))
            {
                GUI.enabled = Platforms.Exists(x => x.Enabled && x.TargetPlatform == BuildTarget.WebPlayer);
                EditorUserBuildSettings.webPlayerStreamed = EditorGUILayout.Toggle("Streamed", EditorUserBuildSettings.webPlayerStreamed);
                EditorUserBuildSettings.webPlayerOfflineDeployment = EditorGUILayout.Toggle("Offline Development", EditorUserBuildSettings.webPlayerOfflineDeployment);


                GUI.enabled = true;
            }
        }
#endif

        /// <summary>Displays build settings for WebGL.</summary>
        private void DisplayWebGLSettings()
        {
            if (platforms.Exists(x => x.TargetPlatform == BuildTarget.WebGL))
            {
#if !UNITY_5_4_OR_NEWER
                //WebGL
                GUI.enabled = Platforms.Exists(x => x.Enabled && x.TargetPlatform == BuildTarget.WebGL);
                EditorUserBuildSettings.webGLOptimizationLevel = (int)((WebOptimisation)EditorGUILayout.EnumPopup("WebGL Optimisation", (WebOptimisation)EditorUserBuildSettings.webGLOptimizationLevel));
#endif

                GUI.enabled = true;
            }
        }

#if UNITY_5_6_OR_NEWER
        /// <summary>Displays build settings for Facebook.</summary>
        private void DisplayFBSettings()
        {
#if !UNITY_2019_3_OR_NEWER
            //Facebook
            if (platforms.Exists(x => x.TargetGroup == BuildTargetGroup.Facebook))
            {

                GUI.enabled = true;
            }
#endif
        }
#endif

            /// <summary>Displays build settings for XboxOne.</summary>
            private void DisplayXboxOneSettings()
        {
            if (platforms.Exists(x => x.TargetGroup == BuildTargetGroup.XboxOne))
            {
                bool XboxOneEnabled = platforms.Exists(x => x.TargetGroup == BuildTargetGroup.XboxOne && x.Enabled);
                GUI.enabled = XboxOneEnabled;
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(new GUIContent("Xbox One Settings", "All settings unique to the Xbox One platform will be found under this header."));
                EditorUserBuildSettings.xboxBuildSubtarget = (XboxBuildSubtarget)EditorGUILayout.EnumPopup(new GUIContent("Build Type", "The Xbox One build subtarget."), EditorUserBuildSettings.xboxBuildSubtarget);
                EditorUserBuildSettings.xboxOneDeployMethod = (XboxOneDeployMethod)EditorGUILayout.EnumPopup(new GUIContent("Deploy Method", "The Xbox One deploy method to be used."), EditorUserBuildSettings.xboxOneDeployMethod);
                GUI.enabled &= sceneList.Count > 1;
                const string LaunchRangeTooltip = "When building an Xbox One Streaming Install package (makepkg.exe) The layout generation code in Unity will assign each scene and associated assets to individual chunks. Unity will mark scene 0 as being part of the launch range, IE the set of chunks required to launch the game, you may include additional scenes in this launch range if you desire, this specifies a range of scenes (starting at 0) to be included in the launch set.";
                EditorUserBuildSettings.streamingInstallLaunchRange = Mathf.Clamp(EditorGUILayout.IntField(new GUIContent("Launch Scene Range", LaunchRangeTooltip), EditorUserBuildSettings.streamingInstallLaunchRange), 0, Mathf.Max(0, sceneList.Count - 1));
                GUI.enabled = XboxOneEnabled;
#if UNITY_2018_2_OR_NEWER
                EditorUserBuildSettings.xboxOneDeployDrive = (XboxOneDeployDrive)EditorGUILayout.EnumPopup(new GUIContent("Deploy Drive", "The Xbox One deploy method to be used."), EditorUserBuildSettings.xboxOneDeployDrive);
#endif
                GUI.enabled = true;
            }
        }

        /// <summary>Displays sub-menu for Scripting Define Symbols.</summary>
        private void DisplayPreprocessorsSettings()
        {
            //Scripting define symbols
            PreprocessorsFoldout = EditorGUILayout.Foldout(PreprocessorsFoldout, new GUIContent("Scripting Define Symbols (Preprocessors)", "Which scripting define symbols to use when compiling the builds."), BoldFoldout);
            if (PreprocessorsFoldout)
            {
                //Universal
                GUI.enabled = platforms.Exists(x => x.Enabled);
                UniDefine = EditorGUILayout.TextField(new GUIContent("Universal", "The scripting define symbols that will be used for all platforms."), UniDefine);

                //Development
                GUI.enabled = Dev;
                DevDefine = EditorGUILayout.TextField(new GUIContent("Development", "The scripting define symbols that will be used for development builds."), DevDefine);

                //Platforms
                foreach (Platform Target in platforms)
                {
                    GUI.enabled = Target.Enabled;
                    Target.UpdateScriptingDefines(this);
                    string FullDefine = Target.FullDefineSymbols.Replace(" ", "").Replace(";", "\n");
                    Target.ScriptingDefine = EditorGUILayout.TextField(new GUIContent(Target.Name, "The scripting define symbols that will be used for " + Target.Name + "." + (String.IsNullOrEmpty(Target.FullDefineSymbols) ? "" : "\nFull symbol list:\n" + FullDefine)), Target.ScriptingDefine);
                }

                EditorGUILayout.Space();
            }

            GUI.enabled = true;
        }

        /// <summary>Displays sub-menu for Batch Scripts.</summary>
        private void DisplayBatchSettings()
        {
            string CurrentPath;

            //Batch scripts
            BatchFoldout = EditorGUILayout.Foldout(BatchFoldout, new GUIContent("Batch/Shell Scripts", "Which platforms to run batch scripts for and the corresponding paths to those scripts."), BoldFoldout);
            if (BatchFoldout)
            {
                //Global toggle
                GUI.enabled = platforms.Exists(x => x.Enabled);
                Batch = EditorGUILayout.BeginToggleGroup(new GUIContent("Enable Batch Scripts", "Run batch scripts after each build."), Batch);

                //Platform-wise
                foreach (Platform Target in platforms)
                {
                    //Platform toggle
                    GUI.enabled = Target.Enabled && Batch;
                    Target.BatchEnabled = EditorGUILayout.BeginToggleGroup(new GUIContent(Target.Name, "Runs a batch script after building " + Target.Name), Target.BatchEnabled);

                    //Path input system per Platform
                    CurrentPath = PathHandling.Combine(BuildPath, Target.BatchPath);
                    GUILayout.BeginHorizontal();
                    Target.BatchPath = EditorGUILayout.TextField(new GUIContent("File Path", "Path to the batch script for the " + Target.Name + " Platform." + FinaliseBAString((String.IsNullOrEmpty(Target.BatchPath) ? "" : ("\nCurrent path: " + CurrentPath)))), Target.BatchPath);
                    if (GUILayout.Button(new GUIContent(FolderIcon, "File browser for selecting the desired batch script for " + Target.Name + "."), FolderStyle))
                    {
                        string Selection = EditorUtility.OpenFilePanel(Target.Name + " Batch Script", RootPath, "");
                        if (!String.IsNullOrEmpty(Selection))
                        {
                            if (DesiredPathType == PathType.Absolute) { Target.BatchPath = Selection.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar); }
                            else
                            {
                                try { Target.BatchPath = PathHandling.MakeRelative(RootPath, Selection); }
                                catch { Debug.LogError("Could not convert " + Selection.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar) + " to be relative to " + RootPath + ". Have you considered using Absolute path mode instead?"); }
                            }
                        }
                    }

                    GUILayout.EndHorizontal();
                    EditorGUILayout.EndToggleGroup();
                }

                //Universal batch paths
                GUI.enabled = Batch;
                CurrentPath = PathHandling.Combine(BuildPath, UniversalBatchPath);
                UniBatch = EditorGUILayout.BeginToggleGroup(new GUIContent("Universal", "Runs a universal batch script, which will run at the end of the entire build pipeline."), UniBatch);
                //Path input system for universal
                GUILayout.BeginHorizontal();
                UniversalBatchPath = EditorGUILayout.TextField(new GUIContent("File Path", "Path to the universal batch script." + FinaliseBAString((String.IsNullOrEmpty(UniversalBatchPath) ? "" : ("\nCurrent path: " + CurrentPath)))), UniversalBatchPath);
                if (GUILayout.Button(new GUIContent(FolderIcon, "File browser for selecting the desired universal batch script."), FolderStyle))
                {
                    string Selection = EditorUtility.OpenFilePanel("Universal Batch Script", RootPath, "");
                    if (!String.IsNullOrEmpty(Selection))
                    {
                        if (DesiredPathType == PathType.Absolute) { UniversalBatchPath = Selection.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar); }
                        else
                        {
                            try { UniversalBatchPath = PathHandling.MakeRelative(RootPath, Selection); }
                            catch { Debug.LogError("Could not convert " + Selection.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar) + " to be relative to " + RootPath + ". Have you considered using Absolute path mode instead?"); }
                        }
                    }
                }
                GUILayout.EndHorizontal();
                EditorGUILayout.EndToggleGroup();

                EditorGUILayout.EndToggleGroup();
                EditorGUILayout.Space();
            }

            GUI.enabled = true;
        }

        /// <summary>Displays sub-menu for Other Settings.</summary>
        private void DisplayOtherSettings()
        {
            //Other settings
            OtherFoldout = EditorGUILayout.Foldout(OtherFoldout, new GUIContent("Other Settings", "Various settings that do not affect the build pipeline."), BoldFoldout);
            if (OtherFoldout)
            {
                Revert = EditorGUILayout.Toggle(new GUIContent("Revert Platform", "Switches back to the original Platform (" + CurrentPlatform.Name + ") after completing the builds."), Revert);
                SaveOnBuild = EditorGUILayout.Toggle(new GUIContent("Save on Build", "Automatically saves the current scene if it will be included in the builds."), SaveOnBuild);
            }
        }

        /// <summary>Main button to initiate build sequence.</summary>
        /// <returns>If the button was pressed and settings were successfully validated.</returns>
        private bool DisplayBuildButton()
        {
            //Error validation
            string Errors = "";
            if (!platforms.Exists(x => x.Enabled)) { GUI.enabled = false; Errors += "\nAt least one Platform must be selected."; }
            if (BuildName == "") { GUI.enabled = false; Errors += "\nA build name must be specified."; }
            if (!sceneList.ValidSceneList) { GUI.enabled = false; Errors += "\nAt least one scene must be chosen."; }

            //Main button
            return GUILayout.Button(new GUIContent("Build Project", "Initiate the Build Automator pipeline and build all of the selected platforms." + Errors));
        }

        /// <summary>Main button to initiate preset pipeline build sequence.</summary>
        /// <returns>If the button was pressed and settings were successfully validated.</returns>
        private bool DisplayPresetBuildButton()
        {
            //Error validation
            string Errors = "";
            if (!platforms.Exists(x => x.Enabled)) { GUI.enabled = false; Errors += "\nAt least one Platform must be selected."; }
            if (BuildName == "") { GUI.enabled = false; Errors += "\nA build name must be specified."; }
            if (!sceneList.ValidSceneList) { GUI.enabled = false; Errors += "\nAt least one scene must be chosen."; }
            if (presetPipelineList.Count == 0) { GUI.enabled = false; Errors += "\nAt least one preset must be added to the pipeline."; }

            //Main button
            return GUILayout.Button(new GUIContent("Execute Preset Pipeline", "Initiate the preset pipeline automation and build each preset." + Errors));
        }
    }
}
