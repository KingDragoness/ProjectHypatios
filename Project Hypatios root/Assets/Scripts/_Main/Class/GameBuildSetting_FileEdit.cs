using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using Sirenix.OdinInspector;

[System.Serializable]
public class GameBuildSettings
{
    public string SettingName = "Demo";
    public bool DemoBuild = false;
    [ShowIf("DemoBuild", true)] public int TrialTimeLimit = 5400;

    public static readonly string StreamingAssetsPath = Application.streamingAssetsPath;
    public static readonly string DemoBuildPath = StreamingAssetsPath + "/BuildGameSettings_DemoBuild.setting";
    public static readonly string FullBuildPath = StreamingAssetsPath + "/BuildGameSettings_FullBuild.setting";
    public static readonly string MainBuildPath = StreamingAssetsPath + "/BuildGameSettings.setting";

    public static bool IsDemoMode()
    {
        var buildSettingDat = JsonConvert.DeserializeObject<GameBuildSettings>(File.ReadAllText(MainBuildPath), FPSMainScript.JsonSettings());

        if (buildSettingDat.DemoBuild == true)
            return true;

        return false;
    }

    public static GameBuildSettings GetMainBuildSettings()
    {
        var buildSettingDat = JsonConvert.DeserializeObject<GameBuildSettings>(File.ReadAllText(MainBuildPath), FPSMainScript.JsonSettings());

        return buildSettingDat;
    }
}

public class GameBuildSetting_FileEdit : MonoBehaviour
{

    public GameBuildSettings demoSettings;
    public GameBuildSettings releaseSettings;

 

    [Button("Save Setting-DEMO BUILD")]
    public void SaveDemoBuildSetting()
    {
        JsonSerializerSettings settings = FPSMainScript.JsonSettings();
        var buildSettingDat = demoSettings;

        string jsonTypeNameAll = JsonConvert.SerializeObject(buildSettingDat, Formatting.Indented, settings);
        File.WriteAllText(GameBuildSettings.DemoBuildPath, jsonTypeNameAll);

    }

    [Button("Save Setting-FULL BUILD")]
    public void SaveReleaseBuildSetting()
    {
        JsonSerializerSettings settings = FPSMainScript.JsonSettings();
        var buildSettingDat = releaseSettings;

        string jsonTypeNameAll = JsonConvert.SerializeObject(buildSettingDat, Formatting.Indented, settings);
        File.WriteAllText(GameBuildSettings.FullBuildPath, jsonTypeNameAll);

    }
}
