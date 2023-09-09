using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System;

namespace QFSW.BA
{
    /// <summary>Determines which backends are compatible for different build targets.</summary>
    public static class BackendCompatibility
    {
        /// <summary>Gets if a platform is supported with legacy support.</summary>
        /// <returns>If it is supported on this unity installation.</returns>
        /// <param name="Target">The build target.</param>
        /// <param name="Group">The build target group.</param>
        public static bool GetPlatformIsSupported(BuildTarget Target, BuildTargetGroup Group)
        {
#if UNITY_2018_1_OR_NEWER
            return BuildPipeline.IsBuildTargetSupported(Group, Target);
#else
            //Legacy method
            Type ModuleManager = System.Type.GetType("UnityEditor.Modules.ModuleManager,UnityEditor.dll");
            MethodInfo IsPlatformSupportLoaded = ModuleManager.GetMethod("IsPlatformSupportLoaded", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            MethodInfo GetTargetStringFromBuildTarget = ModuleManager.GetMethod("GetTargetStringFromBuildTarget", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            return Convert.ToBoolean(IsPlatformSupportLoaded.Invoke(null, new object[] { (string)GetTargetStringFromBuildTarget.Invoke(null, new object[] { Target }) }));
#endif
        }

        /// <summary>Gets all compatible backends for this target.</summary>
        /// <param name="Target">The build target.</param>
        /// <param name="Group">The build target group.</param>
        /// <returns>Array of compatible backends.</returns>
        public static ScriptingImplementation[] GetCompatibleBackends(BuildTarget Target, BuildTargetGroup Group)
        {
            List<ScriptingImplementation> Backends = new List<ScriptingImplementation>();
            if (GetPlatformIsSupported(Target, Group))
            {
                if (SupportsMono(Target, Group)) { Backends.Add(ScriptingImplementation.Mono2x); }
                if (SupportsNET(Target, Group)) { Backends.Add(ScriptingImplementation.WinRTDotNET); }
                if (SupportsIL2CPP(Target, Group)) { Backends.Add(ScriptingImplementation.IL2CPP); }
            }
            return Backends.ToArray();
        }

        /// <summary>Determines if a build target supports the Mono backend.</summary>
        /// <param name="Target">The build target.</param>
        /// <param name="Group">The build target group.</param>
        /// <returns>If the Mono backend is supported.</returns>
        private static bool SupportsMono(BuildTarget Target, BuildTargetGroup Group)
        {
            switch (Group)
            {
                case BuildTargetGroup.iOS:
                case BuildTargetGroup.tvOS:
                case BuildTargetGroup.WSA:
                case BuildTargetGroup.WebGL:
                case BuildTargetGroup.XboxOne:
                    return false;

                default:
                    return true;
            }
        }

        /// <summary>Determines if a build target supports the IL2CPP backend.</summary>
        /// <param name="Target">The build target.</param>
        /// <param name="Group">The build target group.</param>
        /// <returns>If the IL2CPP backend is supported.</returns>
        private static bool SupportsIL2CPP(BuildTarget Target, BuildTargetGroup Group)
        {
            switch (Group)
            {
                case BuildTargetGroup.Standalone:
                    return SupportsIL2CPPStandalone(Target);

                case BuildTargetGroup.Android:
                case BuildTargetGroup.iOS:
                case BuildTargetGroup.tvOS:
                case BuildTargetGroup.WebGL:
#if UNITY_5_5_OR_NEWER && !UNITY_2018_1_OR_NEWER
                case BuildTargetGroup.N3DS:
#endif
                case BuildTargetGroup.Switch:
                case BuildTargetGroup.PS4:
#if !UNITY_2018_3_OR_NEWER
                case BuildTargetGroup.PSP2:
#endif
                case BuildTargetGroup.XboxOne:
                    return true;

                case BuildTargetGroup.WSA:
                    return IsIL2CPPInstalled(Target);

                default:
                    return false;
            }
        }

        /// <summary>Determines if a build target supports the .NET backend.</summary>
        /// <param name="Target">The build target.</param>
        /// <param name="Group">The build target group.</param>
        /// <returns>If the .NET backend is supported.</returns>
        private static bool SupportsNET(BuildTarget Target, BuildTargetGroup Group)
        {
            switch (Group)
            {
                case BuildTargetGroup.WSA:
                    return IsNETInstalled(Target);

                default:
                    return false;
            }
        }

        /// <summary>Determines if the standalone build target supports the IL2CPP backend.</summary>
        /// <param name="Target">The standalone build target.</param>
        /// <returns>If the IL2CPP backend is supported.</returns>
        private static bool SupportsIL2CPPStandalone(BuildTarget Target)
        {
#if UNITY_2018_1_OR_NEWER
            switch (Target)
            {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    {
                        if (Application.platform == RuntimePlatform.WindowsEditor) { return IsIL2CPPInstalled(Target); }
                        else { return false; }
                    }

                case BuildTarget.StandaloneLinux64:
#if !UNITY_2019_2_OR_NEWER
                case BuildTarget.StandaloneLinux:
                case BuildTarget.StandaloneLinuxUniversal:
#endif
                    {
                        if (Application.platform == RuntimePlatform.LinuxEditor) { return IsIL2CPPInstalled(Target); }
                        else { return false; }
                    }

                case BuildTarget.StandaloneOSX:
                    {
                        if (Application.platform == RuntimePlatform.OSXEditor) { return IsIL2CPPInstalled(Target); }
                        else { return false; }
                    }

                default:
                    return false;
            }
#else
                    return false;
#endif
        }

        private static string GetPlayerPackage(BuildTarget target)
        {
#if !UNITY_2019_1_OR_NEWER
            MethodInfo[] methods = typeof(BuildPipeline).GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            MethodInfo getPlaybackEngineDirectory = methods.First((MethodInfo x) => x.Name == "GetPlaybackEngineDirectory" && x.ReturnType == typeof(string) && x.GetParameters().Length == 2);
            return getPlaybackEngineDirectory.Invoke(null, new object[] { target, BuildOptions.None }).ToString();
#else
            return BuildPipeline.GetPlaybackEngineDirectory(target, BuildOptions.None);
#endif
        }

        /// <summary>Determines if the IL2CPP backend is installed.</summary>
        /// <param name="Target">The build target.</param>
        /// <returns>If the IL2CPP backend is installed for this target.</returns>
        private static bool IsIL2CPPInstalled(BuildTarget Target)
        {
            string PlayerPackage = GetPlayerPackage(Target);
            string PlayerName;
            string[] IL2CPPVariations;

            switch (Target)
            {
#if UNITY_2018_1_OR_NEWER
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    {
                        PlayerName = "UnityPlayer.dll";

                        if (Target == BuildTarget.StandaloneWindows)
                        {
                            IL2CPPVariations = new string[]
                            {
                               "win32_development_il2cpp",
                               "win32_nondevelopment_il2cpp"
                            };
                        }
                        else
                        {
                            IL2CPPVariations = new string[]
                            {
                               "win64_development_il2cpp",
                               "win64_nondevelopment_il2cpp"
                            };
                        }

                        break;
                    }

                case BuildTarget.StandaloneLinux64:
#if !UNITY_2019_2_OR_NEWER
                case BuildTarget.StandaloneLinux:
                case BuildTarget.StandaloneLinuxUniversal:
#endif
                    {
                        PlayerName = "LinuxPlayer";

#if !UNITY_2019_2_OR_NEWER
                        if (Target == BuildTarget.StandaloneLinux)
                        {
                            IL2CPPVariations = new string[]
                            {
                                "linux32_headless_development_il2cpp",
                                "linux32_headless_nondevelopment_il2cpp",
                                "linux32_withgfx_development_il2cpp",
                                "linux32_withgfx_nondevelopment_il2cpp"
                            };
                        }
                        else
#endif
                        {
                            IL2CPPVariations = new string[]
                            {
                                "linux64_headless_development_il2cpp",
                                "linux64_headless_nondevelopment_il2cpp",
                                "linux64_withgfx_development_il2cpp",
                                "linux64_withgfx_nondevelopment_il2cpp",
                            };
                        }

                        break;
                    }

                case BuildTarget.StandaloneOSX:
                    {
                        PlayerName = "UnityPlayer.app/Contents/MacOS/UnityPlayer";

                        IL2CPPVariations = new string[]
                        {
                            "macosx64_development_il2cpp",
                            "macosx64_nondevelopment_il2cpp"
                        };

                        break;
                    }
#endif
                case BuildTarget.WSAPlayer:
                    {
                        PlayerName = "UnityPlayer.dll";

                        IL2CPPVariations = new string[]
                        {
                            "il2cpp/ARM/debug",
                            "il2cpp/ARM/master",
                            "il2cpp/ARM/release",
                            "il2cpp/x86/debug",
                            "il2cpp/x86/master",
                            "il2cpp/x86/release",
                            "il2cpp/x64/debug",
                            "il2cpp/x64/master",
                            "il2cpp/x64/release",
                        };

                        break;
                    }

                default:
                    return false;
            }

            if (Target == BuildTarget.WSAPlayer) { return IL2CPPVariations.ToList().Exists((string x) => File.Exists(Path.Combine(Path.Combine(PlayerPackage, "Players/UAP"), Path.Combine(x, PlayerName)))); }
            else { return IL2CPPVariations.ToList().Exists((string x) => File.Exists(Path.Combine(Path.Combine(PlayerPackage, "Variations"), Path.Combine(x, PlayerName)))); }
        }

        /// <summary>Determines if the .NET backend is installed.</summary>
        /// <param name="Target">The build target.</param>
        /// <returns>If the .NET backend is installed for this target.</returns>
        private static bool IsNETInstalled(BuildTarget Target)
        {
            string PlayerPackage = GetPlayerPackage(Target);
            string PlayerName;
            string[] NETVariations;

            switch (Target)
            {
                case BuildTarget.WSAPlayer:
                    {
                        PlayerName = "UnityPlayer.dll";

                        NETVariations = new string[]
                        {
                            "dotnet/ARM/debug",
                            "dotnet/ARM/master",
                            "dotnet/ARM/release",
                            "dotnet/x86/debug",
                            "dotnet/x86/master",
                            "dotnet/x86/release",
                            "dotnet/x64/debug",
                            "dotnet/x64/master",
                            "dotnet/x64/release",
                        };

                        break;
                    }

                default:
                    return false;
            }

            return NETVariations.ToList().Exists((string x) => File.Exists(Path.Combine(Path.Combine(PlayerPackage, "Players/UAP"), Path.Combine(x, PlayerName))));
        }
    }
}
