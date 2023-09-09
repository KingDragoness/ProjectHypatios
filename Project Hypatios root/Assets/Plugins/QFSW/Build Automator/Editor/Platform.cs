using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEditor;
#if UNITY_2018_1_OR_NEWER
using UnityEditor.Build;
#endif

namespace QFSW.BA
{
    /// <summary>Object to hold all of the settings and information for each Platform.</summary>
    public class Platform
    {
        /// <summary>The state of the Platform.</summary>
        public enum PlatformState
        {
            /// <summary>No special state.</summary>
            None = 0,
            /// <summary>Queued for build.</summary>
            Queued = 1,
            /// <summary>Unsuccessfully built to.</summary>
            Unsuccessful = 2,
            /// <summary>Successfully built to.</summary>
            Successful = 3,
            /// <summary>Currently active Platform.</summary>
            Active = 4
        }

        //Details and settings for Platform
        /// <summary>The name of the Platform.</summary>
        public readonly string Name;
        /// <summary>If this Platform will be built when Build Automator runs.</summary>
        public bool Enabled;
        /// <summary>If this Platform will run a batch script when Build Automator runs.</summary>
        public bool BatchEnabled;
        /// <summary>The path to this Platform's batch script.</summary>
        public string BatchPath;
        /// <summary>Any scripting define symbols to associate with this Platform.</summary>
        public string ScriptingDefine;
        /// <summary>The full list of scripting define symbols to use with this Platform.</summary>
        public string FullDefineSymbols;
        /// <summary>The subfolder to place this Platform's builds in.</summary>
        public string Folder;
        /// <summary>The displayed message whilst this Platform is building.</summary>
        public string BuildMessage;
        /// <summary>The path that this Platform will build its files to.</summary>
        public string BuildPath;
        /// <summary>Prefix to prepend to the build name when building to this Platform.</summary>
        public string NamePrefix;
        /// <summary>Suffix to append to the build name when building to this Platform.</summary>
        public string NameSuffix;

        /// <summary>The icon to display alongside this Platform.</summary>
        public Texture Icon;

        /// <summary>The extension for the file/folder name to use for this Platform's builds.</summary>
        public readonly string Extension;
        /// <summary>The BuildTarget associated with this Platform.</summary>
        public readonly BuildTarget TargetPlatform;
        /// <summary>The BuildTargetGroup associated with this Platform.</summary>
        public readonly BuildTargetGroup TargetGroup;
        /// <summary>Options for this build.</summary>
        public BuildOptions PlatformOptions;

        /// <summary>Backends supported by this platform.</summary>
        public readonly ScriptingImplementation[] SupportedBackends;
        /// <summary>Clean string forms of the backends for the GUI.</summary>
        public readonly string[] BackendNames;
        /// <summary>Currently selected backend.</summary>
        public int CurrentBackend
        {
            get { return _CurrentBackend; }
            set { _CurrentBackend = Mathf.Max(0, Mathf.Min(SupportedBackends.Length - 1, value)); }
        }
        private int _CurrentBackend;

        /// <summary>Currently selected backend.</summary>
        public ScriptingImplementation CurrentScriptingBackend
        {
            get
            {
                if (SupportedBackends != null && SupportedBackends.Length > 0) { return SupportedBackends[CurrentBackend]; }
                else { return default(ScriptingImplementation); }
            }
        }

#if UNITY_2018_1_OR_NEWER
        /// <summary>Current C++ compiler configuration to be used by this platform.</summary>
        public Il2CppCompilerConfiguration CppCompilerConfig;
#endif

        /// <summary>Current PlatformState of this Platform.</summary>
        public PlatformState CurrentState = 0;
        /// <summary>If this is the active Platform.</summary>
        public bool IsActive;

        //Constructor
        /// <summary>The constructor for a Platform object, which will contain all the details required to represent a build target.</summary>
        /// <param name="PlatformName">The name of the Platform to be displayed in the editor.</param>
        /// <param name="PlatformExt">The extension to add onto the file or folder name for builds generated in this Platform.</param>
        /// <param name="Message">The message to display to the editor whilst this Platform is generating a build.</param>
        /// <param name="PlatformType">The specific BuildTarget that corresponds to the desired Platform.</param>
        /// <param name="PlatformGroup">The BuildTargetGroup that the desired Platform belongs to.</param>
        /// <param name="FolderName">(Optional) The folder name for this Platform if subfolders are enabled.</param>
        public Platform(string PlatformName, string PlatformExt, string Message, BuildTarget PlatformType, BuildTargetGroup PlatformGroup, string FolderName)
        {
            Name = PlatformName;
            Extension = PlatformExt;
            Folder = FolderName;
            BuildMessage = Message;
            TargetPlatform = PlatformType;
            TargetGroup = PlatformGroup;

            SupportedBackends = BackendCompatibility.GetCompatibleBackends(TargetPlatform, TargetGroup);
            BackendNames = new string[SupportedBackends.Length];
            CurrentBackend = 0;
            for (int i = 0; i < SupportedBackends.Length; i++)
            {
                switch (SupportedBackends[i])
                {
                    case ScriptingImplementation.Mono2x: BackendNames[i] = "Mono"; break;
                    case ScriptingImplementation.IL2CPP: BackendNames[i] = "IL2CPP"; break;
                    case ScriptingImplementation.WinRTDotNET: BackendNames[i] = ".NET"; break;
                }
            }
        }

        /// <summary>The constructor for a Platform object, which will contain all the details required to represent a build target.</summary>
        /// <param name="PlatformName">The name of the Platform to be displayed in the editor.</param>
        /// <param name="PlatformExt">The extension to add onto the file or folder name for builds generated in this Platform.</param>
        /// <param name="Message">The message to display to the editor whilst this Platform is generating a build.</param>
        /// <param name="PlatformType">The specific BuildTarget that corresponds to the desired Platform.</param>
        /// <param name="PlatformGroup">The BuildTargetGroup that the desired Platform belongs to.</param>
        public Platform(string PlatformName, string PlatformExt, string Message, BuildTarget PlatformType, BuildTargetGroup PlatformGroup)
            : this(PlatformName, PlatformExt, Message, PlatformType, PlatformGroup, PlatformName) { }

        /// <summary>The constructor for a Platform object, which will contain all the details required to represent a build target.</summary>
        /// <param name="PlatformName">The name of the Platform to be displayed in the editor.</param>
        /// <param name="PlatformExt">The extension to add onto the file or folder name for builds generated in this Platform.</param>
        /// <param name="Message">The message to display to the editor whilst this Platform is generating a build.</param>
        /// <param name="PlatformType">The specific BuildTarget that corresponds to the desired Platform.</param>
        /// <param name="PlatformGroup">The BuildTargetGroup that the desired Platform belongs to.</param>
        /// <param name="PlatformIcon">(Optional) The icon associated with the Platform.</param>
        public Platform(string PlatformName, string PlatformExt, string Message, BuildTarget PlatformType, BuildTargetGroup PlatformGroup, Texture PlatformIcon)
            : this(PlatformName, PlatformExt, Message, PlatformType, PlatformGroup, PlatformName) { Icon = PlatformIcon; }

        /// <summary>The constructor for a Platform object, which will contain all the details required to represent a build target.</summary>
        /// <param name="PlatformName">The name of the Platform to be displayed in the editor.</param>
        /// <param name="PlatformExt">The extension to add onto the file or folder name for builds generated in this Platform.</param>
        /// <param name="Message">The message to display to the editor whilst this Platform is generating a build.</param>
        /// <param name="PlatformType">The specific BuildTarget that corresponds to the desired Platform.</param>
        /// <param name="PlatformGroup">The BuildTargetGroup that the desired Platform belongs to.</param>
        /// <param name="FolderName">(Optional) The folder name for this Platform if Subfolders are enabled.</param>
        /// <param name="PlatformIcon">(Optional) The icon associated with the Platform.</param>
        public Platform(string PlatformName, string PlatformExt, string Message, BuildTarget PlatformType, BuildTargetGroup PlatformGroup, string FolderName, Texture PlatformIcon)
            : this(PlatformName, PlatformExt, Message, PlatformType, PlatformGroup, FolderName) { Icon = PlatformIcon; }

        /// <summary>Generates the output path for this Platform.</summary>
        /// <param name="Controller">The BuildAutomator class in control.</param>
        /// <returns>Completed absolute build path.</returns>
        public string UpdateBuildPath(BuildAutomator Controller)
        {
            BuildPath = Controller.BuildPath;
            if (!String.IsNullOrEmpty(Controller.OutputPath)) { BuildPath = PathHandling.Combine(BuildPath, Controller.OutputPath); }
            if (Controller.Subfolders) { BuildPath = PathHandling.Combine(BuildPath, Folder); }
            BuildPath = Controller.FinaliseBAString(PathHandling.Combine(BuildPath, NamePrefix + Controller.BuildName + NameSuffix + Extension));
            return BuildPath;
        }

        /// <summary>Generates the full string of scripting define symbols for this Platform.</summary>
        /// <param name="Controller">The BuildAutomator class in control.</param>
        /// <returns>Full string of scripting define symbols for this Platform.</returns>
        public string UpdateScriptingDefines(BuildAutomator Controller)
        {
            //Creates string
            FullDefineSymbols = Controller.UniDefine;
            if (Controller.Dev && !String.IsNullOrEmpty(Controller.DevDefine)) { FullDefineSymbols += (String.IsNullOrEmpty(FullDefineSymbols) ? "" : ";") + Controller.DevDefine; }
            if (!String.IsNullOrEmpty(ScriptingDefine)) { FullDefineSymbols += (String.IsNullOrEmpty(FullDefineSymbols) ? "" : ";") + ScriptingDefine; }

            //Strips off extra chars
            FullDefineSymbols = FullDefineSymbols.Replace(" ", "");
            while (FullDefineSymbols != FullDefineSymbols.Replace(";;", ";")) { FullDefineSymbols = FullDefineSymbols.Replace(";;", ";"); }
            FullDefineSymbols = FullDefineSymbols.Trim(';');
            FullDefineSymbols = FullDefineSymbols.Trim();

            return FullDefineSymbols;
        }

        /// <summary>Flushes player settings with updated ones specific to this platform.</summary>
        private void FlushPlayerSettings()
        {
            PlayerSettings.SetScriptingBackend(TargetGroup, CurrentScriptingBackend);
#if UNITY_2018_1_OR_NEWER
            if (CurrentScriptingBackend == ScriptingImplementation.IL2CPP) { PlayerSettings.SetIl2CppCompilerConfiguration(TargetGroup, CppCompilerConfig); }
#endif
        }

        /// <summary>Attempts to switch to this Platform.</summary>
        /// <returns>If the switch was successful.</returns>
        public bool SwitchToPlatform()
        {
            //Attempts to switch
            bool Success;
#if UNITY_5_6_OR_NEWER
            Success = EditorUserBuildSettings.SwitchActiveBuildTarget(TargetGroup, TargetPlatform);
#else
            Success = EditorUserBuildSettings.SwitchActiveBuildTarget(TargetPlatform);
#endif
            //Marks as active
            if (Success)
            {
                FlushPlayerSettings();
                CurrentState = PlatformState.Active;
                IsActive = true;

                //Updates Databases
                AssetDatabase.Refresh();
            }

            return Success;
        }

        /// <summary>Attempts to switch to this Platform.</summary>
        /// <param name="OldPlatform">The currently active Platform.</param>
        /// <returns>If the switch was successful.</returns>
        public bool SwitchToPlatform(Platform OldPlatform)
        {
            //Marks old Platform as inactive if successful
            if (SwitchToPlatform() && OldPlatform != this)
            {
                if (OldPlatform.CurrentState == PlatformState.Active) { OldPlatform.CurrentState = PlatformState.None; }
                OldPlatform.IsActive = false;
                return true;
            }
            else { return false; }
        }

#if UNITY_2018_1_OR_NEWER
        /// <summary>Dumps the build information to the log file.</summary>
        /// <param name="LogWriter">The StreamWriter for the current log file.</param>
        /// <param name="BuildLog">The BuildLog to dump the file.</param>
        private void DumpToLog(System.IO.StreamWriter LogWriter, UnityEditor.Build.Reporting.BuildReport BuildLog)
        {
            //Gets the build size in the appropriate units
            string SizeUnits;
            float ConvertedSize;
            ulong BuildSize = BuildLog.summary.totalSize;
            if (BuildSize < 1000)
            {
                SizeUnits = "B";
                ConvertedSize = BuildSize;
            }
            else if (BuildSize < 1000000)
            {
                SizeUnits = "KB";
                ConvertedSize = BuildSize / 1000f;
            }
            else if (BuildSize < 1000000000)
            {
                SizeUnits = "MB";
                ConvertedSize = BuildSize / 1000000f;
            }
            else
            {
                SizeUnits = "GB";
                ConvertedSize = BuildSize / 1000000000f;
            }
            if (BuildSize >= 1000)
            {
                double NumberScale = Math.Pow(10, Math.Floor(Math.Log10(ConvertedSize)) + 1);
                ConvertedSize = (float) (NumberScale * Math.Round(ConvertedSize / NumberScale, 3));
            }

            //Writes to the logger
            LogWriter.WriteLine("Platform: " + Name);
            LogWriter.WriteLine("Build State: " + BuildLog.summary.result.ToString());
            LogWriter.WriteLine("Completion Time: " + BuildLog.summary.totalTime.ToString());
            LogWriter.WriteLine("Total Build Size: " + ConvertedSize.ToString() + SizeUnits);
            LogWriter.WriteLine("Total Errors: " + BuildLog.summary.totalErrors.ToString());
            LogWriter.WriteLine("Total Warnings: " + BuildLog.summary.totalWarnings.ToString());
            LogWriter.WriteLine("");
            LogWriter.WriteLine("");
        }
#endif

        /// <summary>Begins the build to the current Platform.</summary>
        /// <param name="Controller">The BuildAutomator class in control.</param>
        /// <returns>If the build was successful.</returns>
        public bool BuildToPlatform(BuildAutomator Controller)
        {
            //Updates info
            Controller.UpdateProgress(BuildMessage);
            UpdateBuildPath(Controller);
            UpdateScriptingDefines(Controller);

            //Sets to development builds if dev is selected
            PlatformOptions = BuildOptions.None;
            if (Controller.Dev) { PlatformOptions |= BuildOptions.Development; }
            if (EditorUserBuildSettings.enableHeadlessMode) { PlatformOptions |= BuildOptions.EnableHeadlessMode; }

#if UNITY_2017_1_OR_NEWER
            switch (Controller.CompressionType)
            {
                case CompressionOptions.LZ4: PlatformOptions |= BuildOptions.CompressWithLz4; break;
#if UNITY_2017_2_OR_NEWER
                case CompressionOptions.LZ4HC: PlatformOptions |= BuildOptions.CompressWithLz4HC; break;
#else
                case CompressionOptions.LZ4HC: PlatformOptions |= BuildOptions.CompressWithLz4; break;
#endif
            }
#endif

            //Sets scripting defines
            PlayerSettings.SetScriptingDefineSymbolsForGroup(TargetGroup, FullDefineSymbols);

            if (SwitchToPlatform())
            {
                //Sets as current Platform
                Controller.CurrentPlatform = this;

                //Builds to correct target
                try
                {
#if UNITY_2018_1_OR_NEWER
                    //Builds Platform
                    BuildPlayerOptions BuildOptions = new BuildPlayerOptions();
                    BuildOptions.locationPathName = BuildPath;
                    BuildOptions.options = PlatformOptions;
                    BuildOptions.scenes = Controller.scenes;
                    BuildOptions.target = TargetPlatform;
                    BuildOptions.targetGroup = TargetGroup;
                    UnityEditor.Build.Reporting.BuildReport BuildLog = BuildPipeline.BuildPlayer(BuildOptions);

                    if (Controller.CurrentLogWriter != null) { DumpToLog(Controller.CurrentLogWriter, BuildLog); }

                    //Error Handling
                    if (BuildLog.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded) { CurrentState = PlatformState.Successful; }
                    else
                    {
                        Debug.LogError("Unable to build to " + Name + ". Build state: " + BuildLog.summary.result.ToString() + ". Errors: " + BuildLog.summary.totalErrors.ToString());
                        CurrentState = PlatformState.Unsuccessful;
                        return false;
                    }
#else
                    //Builds Platform
                    string Error = BuildPipeline.BuildPlayer(Controller.Scenes, BuildPath, TargetPlatform, PlatformOptions);

                    //Error Handling
                    if (String.IsNullOrEmpty(Error)) { CurrentState = PlatformState.Successful; }
                    else
                    {
                        Debug.LogError("Unable to build to " + Name + ".");
                        CurrentState = PlatformState.Unsuccessful;
                        return false;
                    }
#endif
                }
                //Error Handling
                catch
                {
                    Debug.LogError("Unable to build to " + Name + ".");
                    CurrentState = PlatformState.Unsuccessful;
                    return false;
                }

                //Runs batch scripts
                if (Controller.Batch && BatchEnabled && !String.IsNullOrEmpty(BatchPath))
                {
                    Controller.UpdateProgress("Running " + Name + " Command Line Scripts");
                    string CurrentBatchPath = Controller.FinaliseBAString(PathHandling.Combine(Controller.BuildPath, BatchPath));
                    try
                    {
#if UNITY_EDITOR_WIN
                        System.Diagnostics.Process.Start(CurrentBatchPath);
#elif UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
                        System.Diagnostics.Process BatchProcess = new System.Diagnostics.Process();
                        BatchProcess.StartInfo.FileName = "/bin/bash";
                        BatchProcess.StartInfo.Arguments = "-c \" open " + PathHandling.Combine(Controller.BuildPath, BatchPath) + " \"";
                        BatchProcess.StartInfo.UseShellExecute = false;
                        BatchProcess.StartInfo.RedirectStandardOutput = true;
                        BatchProcess.Start();
#endif
                    }
                    catch { Debug.LogError("Unable to start batch script for " + Name + " at " + CurrentBatchPath + "."); }
                }
            }
            else
            {
                //Reports error for failure
                Debug.LogError("Unable to switch to " + Name + ".");
                CurrentState = PlatformState.Unsuccessful;
                return false;
            }

            return true;
        }

    }
}
