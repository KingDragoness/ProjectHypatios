using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;
using Sirenix.OdinInspector;

public class SettingsUI : MonoBehaviour
{

    [FoldoutGroup("Video")] public List<Button> qualityLevels = new List<Button>();
    [FoldoutGroup("Sounds")] public Text value_SFX;
    [FoldoutGroup("Sounds")] public Slider slider_SFX;
    [FoldoutGroup("Sounds")] public Text value_Music;
    [FoldoutGroup("Sounds")] public Slider slider_Music;
    [FoldoutGroup("Gameplay")] public Text value_MouseSensitivity;
    [FoldoutGroup("Gameplay")] public Slider slider_MouseSensitivity;
    [FoldoutGroup("Gameplay")] public Text value_Brightness;
    [FoldoutGroup("Gameplay")] public Slider slider_Brightness;
    [FoldoutGroup("Video")] public Toggle toggle_VSync;
    [FoldoutGroup("Video")] public Toggle toggle_MotionBlur;
    [FoldoutGroup("Video")] public Text value_FPSCap;
    [FoldoutGroup("Video")] public Slider slider_FPSCap;
    [FoldoutGroup("Video")] public Dropdown dropdown_Resolution;
    public InputField inputfield_Name;

    [Space]

    [FoldoutGroup("Sounds")] public AudioMixer mixerSFX;
    [FoldoutGroup("Sounds")] public AudioMixer mixerMusic;

    [Space]
    public int currentLevel = 0;

    public static float SFX_VOLUME = 0.5f;
    public static float MUSIC_VOLUME = 0.5f;
    public static float MOUSE_SENSITIVITY = 0.5f;
    public static float BRIGHTNESS = 0.5f; //-1 - 1 -> 0 - 1
    public static int VSYNC = 0;
    public static int MOTIONBLUR = 0;
    public static int MAXIMUM_FRAMERATE = 201;
    public static int RESOLUTION = -1;

    public static string MY_NAME = "Aldrich";

    private ColorGrading colorGrading;
    private AmbientOcclusion AO;
    private ScreenSpaceReflections realtimeReflections;
    private MotionBlur motionBlur;
    private FloatParameter floatParam_Brightness;
    private Resolution[] resolutions;

    private void Start()
    {
        var validRes = Screen.resolutions;
        resolutions = validRes;

        {
            dropdown_Resolution.options.Clear();

            foreach (var res in resolutions)
            {
                Dropdown.OptionData optionData = new Dropdown.OptionData();
                optionData.text = $"{res.width}, {res.height}";
                dropdown_Resolution.options.Add(optionData);
            }
        }

        AO = FPSMainScript.instance.postProcessVolume.profile.GetSetting<AmbientOcclusion>();
        motionBlur = FPSMainScript.instance.postProcessVolume.profile.GetSetting<MotionBlur>();
        realtimeReflections = FPSMainScript.instance.postProcessVolume.profile.GetSetting<ScreenSpaceReflections>();

        {
            var colorGrading_ = FPSMainScript.instance.postProcessVolume_2.profile.GetSetting<ColorGrading>();
            colorGrading = colorGrading_;

            floatParam_Brightness = colorGrading.postExposure;
        }

    
        MY_NAME = LoadPrefKeyString("SETTINGS.MY_NAME", "Aldrich");
        BRIGHTNESS = LoadPrefKeyFloat("SETTINGS.BRIGHTNESS", 0.5f);
        MAXIMUM_FRAMERATE = LoadPrefKeyInt("SETTINGS.MAXIMUM_FRAMERATE", 201);
        RESOLUTION = LoadPrefKeyInt("SETTINGS.RESOLUTION", resolutions.Length - 1);
        MOUSE_SENSITIVITY = LoadPrefKeyFloat("SETTINGS.MOUSE_SENSITIVITY", 10f);
        MOTIONBLUR = LoadPrefKeyInt("SETTINGS.MOTIONBLUR", 1);
        SFX_VOLUME = AssignValuePref("SETTINGS.SFX_VOLUME", 1); //PlayerPrefs.GetFloat("");
        MUSIC_VOLUME = AssignValuePref("SETTINGS.MUSIC_VOLUME", 1); //PlayerPrefs.GetFloat("SETTINGS.MUSIC_VOLUME");
        VSYNC = PlayerPrefs.GetInt("SETTINGS.VSYNC");

        slider_SFX.SetValueWithoutNotify(SFX_VOLUME);
        slider_Music.SetValueWithoutNotify(MUSIC_VOLUME);
        slider_MouseSensitivity.SetValueWithoutNotify(MOUSE_SENSITIVITY);
        slider_Brightness.SetValueWithoutNotify(BRIGHTNESS);
        slider_FPSCap.SetValueWithoutNotify(MAXIMUM_FRAMERATE);
        toggle_VSync.SetIsOnWithoutNotify(IntToBool(VSYNC));
        toggle_MotionBlur.SetIsOnWithoutNotify(IntToBool(MOTIONBLUR));
        dropdown_Resolution.SetValueWithoutNotify(RESOLUTION);
        inputfield_Name.SetTextWithoutNotify(MY_NAME);

        currentLevel = QualitySettings.GetQualityLevel();
        RefreshUI();

    }

    #region KeyPrefs
    private string LoadPrefKeyString(string Keyname, string defaultValue)
    {
        if (PlayerPrefs.HasKey(Keyname))
        {
            return PlayerPrefs.GetString(Keyname);
        }
        else
        {
            return defaultValue;
        }
    }
    private float LoadPrefKeyFloat(string Keyname, float defaultValue)
    {
        if (PlayerPrefs.HasKey(Keyname))
        {
            return PlayerPrefs.GetFloat(Keyname);
        }
        else
        {
            return defaultValue;
        }
    }

    private int LoadPrefKeyInt(string Keyname, int defaultValue)
    {
        if (PlayerPrefs.HasKey(Keyname))
        {
            return PlayerPrefs.GetInt(Keyname);
        }
        else
        {
            return defaultValue;
        }
    }

    private float AssignValuePref(string KeyName, float defaultValue)
    {
        if (PlayerPrefs.HasKey(KeyName))
        {
            return PlayerPrefs.GetFloat(KeyName);
        }
        else
        {
            return defaultValue;
        }
    }

    public bool IntToBool(int a)
    {
        if (a == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion

    public void Tutorial()
    {
        Application.LoadLevel(1);
    }

    public void UpdateSettings()
    {

    }

    public void SetSomething()
    {
        MY_NAME = inputfield_Name.text;
        MAXIMUM_FRAMERATE = Mathf.RoundToInt(slider_FPSCap.value);
        RESOLUTION = Mathf.RoundToInt(dropdown_Resolution.value);
        PlayerPrefs.SetString("SETTINGS.MY_NAME", MY_NAME);
        PlayerPrefs.SetInt("SETTINGS.MAXIMUM_FRAMERATE", MAXIMUM_FRAMERATE);
        PlayerPrefs.SetInt("SETTINGS.RESOLUTION", RESOLUTION);
        PlayerPrefs.Save();
        RefreshUI();

    }

    public void SetQualityLevel(int level)
    {
        currentLevel = level;
        RefreshUI();
    }

    public void SetVolume_SFX()
    {
        SFX_VOLUME = slider_SFX.value;
        PlayerPrefs.SetFloat("SETTINGS.SFX_VOLUME", SFX_VOLUME);
        PlayerPrefs.Save();
        RefreshUI();
    }

    public void SetVolume_Music()
    {
        MUSIC_VOLUME = slider_Music.value;
        PlayerPrefs.SetFloat("SETTINGS.MUSIC_VOLUME", MUSIC_VOLUME);
        PlayerPrefs.Save();
        RefreshUI();
    }

    public void SetMouseSensitivity()
    {
        MOUSE_SENSITIVITY = slider_MouseSensitivity.value;
        PlayerPrefs.SetFloat("SETTINGS.MOUSE_SENSITIVITY", MOUSE_SENSITIVITY);
        PlayerPrefs.Save();
        RefreshUI();
    }

    public void SetVSync()
    {
        VSYNC = toggle_VSync.isOn ? 1 : 0;
        PlayerPrefs.SetInt("SETTINGS.VSYNC", VSYNC);
        PlayerPrefs.Save();
        RefreshUI();
    }

    public void SetMotionBlur()
    {
        MOTIONBLUR = toggle_MotionBlur.isOn ? 1 : 0;
        PlayerPrefs.SetInt("SETTINGS.MOTIONBLUR", MOTIONBLUR);
        PlayerPrefs.Save();
        RefreshUI();
    }


    public void SetBrightness()
    {
        BRIGHTNESS = slider_Brightness.value;
        PlayerPrefs.SetFloat("SETTINGS.BRIGHTNESS", BRIGHTNESS);
        PlayerPrefs.Save();
        RefreshUI();
    }

    public void RefreshForceSettings()
    {

        {
            if (PlayerPrefs.HasKey("SETTINGS.BRIGHTNESS"))
            {
                BRIGHTNESS = PlayerPrefs.GetFloat("SETTINGS.BRIGHTNESS");
            }
            else
            {
                BRIGHTNESS = 0.5f;
            }
        }

        {
            if (PlayerPrefs.HasKey("SETTINGS.MOUSE_SENSITIVITY"))
            {
                MOUSE_SENSITIVITY = PlayerPrefs.GetFloat("SETTINGS.MOUSE_SENSITIVITY");
            }
            else
            {
                MOUSE_SENSITIVITY = 10f;
            }
        }

        SFX_VOLUME = PlayerPrefs.GetFloat("SETTINGS.SFX_VOLUME");
        MUSIC_VOLUME = PlayerPrefs.GetFloat("SETTINGS.MUSIC_VOLUME");

        {
            var colorGrading_ = FPSMainScript.instance.postProcessVolume_2.profile.GetSetting<ColorGrading>();
            colorGrading = colorGrading_;

            floatParam_Brightness = colorGrading.postExposure;
        }

        if (colorGrading != null)
        {
            colorGrading.gamma.value.w = (BRIGHTNESS / 2f);
            floatParam_Brightness.value = (BRIGHTNESS / 2f);
        }

        cameraScript cam = cameraScript.instance;

        if (cam != null)
        {
            cam.mouseSensitivity = MOUSE_SENSITIVITY;
        }

        mixerSFX.SetFloat("Master", Mathf.Log10(SFX_VOLUME) * 20);
        mixerMusic.SetFloat("Master", Mathf.Log10(MUSIC_VOLUME) * 20);

        slider_MouseSensitivity.SetValueWithoutNotify(MOUSE_SENSITIVITY);
        slider_Brightness.SetValueWithoutNotify(BRIGHTNESS);

    }


    private void RefreshUI()
    {
        for(int x = 0; x < qualityLevels.Count; x++)
        {
            var button = qualityLevels[x];
            QualitySettings.SetQualityLevel(currentLevel);

            if (x == currentLevel)
            {
                button.interactable = false;
            }
            else
            {
                button.interactable = true;
            }

        }

        if (AO != null)
        {
            if (currentLevel == 0)
            {
                AO.active = false;
            }
            else
            {
                AO.active = true;
            }
        }

        if (motionBlur != null)
        {
            if (MOTIONBLUR == 0)
            {
                motionBlur.active = false;
                if (FPSMainScript.instance.minorMotionBlur != null) FPSMainScript.instance.minorMotionBlur.enabled = false;
            }
            else
            {
                motionBlur.active = true;
                if (FPSMainScript.instance.minorMotionBlur != null) FPSMainScript.instance.minorMotionBlur.enabled = true;
            }
        }

        if (realtimeReflections != null)
        {
            if (currentLevel != 2)
            {
                realtimeReflections.active = false;
            }
            else
            {
                realtimeReflections.active = true;
            }
        }

        float displayBrightness = (BRIGHTNESS + 1) / 2;

        value_SFX.text = Mathf.RoundToInt(SFX_VOLUME * 100).ToString();
        value_Music.text = Mathf.RoundToInt(MUSIC_VOLUME * 100).ToString();
        value_MouseSensitivity.text = (Mathf.Round(MOUSE_SENSITIVITY * 10)/10).ToString();
        value_Brightness.text = (Mathf.Round(displayBrightness * 10)/10).ToString();

        if (MAXIMUM_FRAMERATE < 201)
        value_FPSCap.text = (Mathf.Round(MAXIMUM_FRAMERATE)).ToString();
        else value_FPSCap.text = "∞".ToString();

        mixerSFX.SetFloat("Master", Mathf.Log10(SFX_VOLUME) * 20);
        mixerMusic.SetFloat("Master", Mathf.Log10(MUSIC_VOLUME) * 20);

        if (MAXIMUM_FRAMERATE >= 201)
        {
            Application.targetFrameRate = -1;
        }
        else
        {
            Application.targetFrameRate = MAXIMUM_FRAMERATE;
        }

        if (RESOLUTION != -1)
        {
            Screen.SetResolution(resolutions[RESOLUTION].width, resolutions[RESOLUTION].height, Screen.fullScreen);
        }

        if (VSYNC == 0)
        {
            QualitySettings.vSyncCount = 0;
        }
        else
        {
            QualitySettings.vSyncCount = 1;
        }

        //colorGrading.gamma.value.x = 1 - (BRIGHTNESS / 3f);
        //colorGrading.gamma.value.y = 1 - (BRIGHTNESS / 3f);
        //colorGrading.gamma.value.z = 1 - (BRIGHTNESS / 3f);

        if (colorGrading != null)
        {
            colorGrading.gamma.value.w = (BRIGHTNESS / 2f);
            floatParam_Brightness.value = (BRIGHTNESS / 2f);
        }

        cameraScript cam = cameraScript.instance;

        if (cam != null)
        {
            cam.mouseSensitivity = MOUSE_SENSITIVITY;
        }

    }

}
