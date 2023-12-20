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
    [FoldoutGroup("Video")] public Dropdown dropdown_Quality;
    [FoldoutGroup("Sounds")] public Text value_SFX;
    [FoldoutGroup("Sounds")] public Slider slider_SFX;
    [FoldoutGroup("Sounds")] public Text value_Music;
    [FoldoutGroup("Sounds")] public Slider slider_Music;
    [FoldoutGroup("Sounds")] public TestAudioVisualizer audioVisualizer;
    [FoldoutGroup("Gameplay")] public Text value_MouseSensitivity;
    [FoldoutGroup("Gameplay")] public Slider slider_MouseSensitivity;
    [FoldoutGroup("Gameplay")] public Text value_Brightness;
    [FoldoutGroup("Gameplay")] public Slider slider_Brightness;
    [FoldoutGroup("Gameplay")] public Text value_FOV;
    [FoldoutGroup("Gameplay")] public Slider slider_FOV;
    [FoldoutGroup("Gameplay")] public Toggle toggle_AutoDialogue;
    [FoldoutGroup("Gameplay")] public Toggle toggle_RunInBackground;
    [FoldoutGroup("Video")] public Toggle toggle_VSync;
    [FoldoutGroup("Video")] public Toggle toggle_Fullscreen;
    [FoldoutGroup("Video")] public Toggle toggle_MotionBlur;
    [FoldoutGroup("Video")] public Toggle toggle_AntiAliasing;
    [FoldoutGroup("Video")] public Toggle toggle_DynamicUIScaling;
    [FoldoutGroup("Video")] public Toggle toggle_UITVEffect;
    [FoldoutGroup("Video")] public Text value_FPSCap;
    [FoldoutGroup("Video")] public Slider slider_FPSCap;
    [FoldoutGroup("Video")] public Text value_Temperature;
    [FoldoutGroup("Video")] public Slider slider_Temperature;
    [FoldoutGroup("Video")] public Text value_Tint;
    [FoldoutGroup("Video")] public Slider slider_Tint;
    [FoldoutGroup("Video")] public Text value_UIScaling;
    [FoldoutGroup("Video")] public Slider slider_UIScaling;
    [FoldoutGroup("Video")] public Dropdown dropdown_Resolution;
    public InputField inputfield_Name;


    private void OnEnable()
    {
        if (MusicPlayer.Instance != null && audioVisualizer != null)
            audioVisualizer.audioSource = MusicPlayer.Instance.musicSource;
    }




    private void Start()
    {


        {
            dropdown_Resolution.options.Clear();

            foreach (var res in Hypatios.Settings.Resolutions)
            {
                Dropdown.OptionData optionData = new Dropdown.OptionData();
                optionData.text = $"{res.width}, {res.height} ({res.refreshRate}Hz)";
                dropdown_Resolution.options.Add(optionData);
            }
        }
    
        slider_SFX.SetValueWithoutNotify(Hypatios.Settings.SFX_VOLUME);
        slider_Music.SetValueWithoutNotify(Hypatios.Settings.MUSIC_VOLUME);
        slider_MouseSensitivity.SetValueWithoutNotify(Hypatios.Settings.MOUSE_SENSITIVITY);
        slider_Brightness.SetValueWithoutNotify(Hypatios.Settings.BRIGHTNESS);
        slider_Temperature.SetValueWithoutNotify(Hypatios.Settings.VISUAL_TEMPERATURE);
        slider_Tint.SetValueWithoutNotify(Hypatios.Settings.VISUAL_TINT);
        slider_FOV.SetValueWithoutNotify(Hypatios.Settings.FOV);
        slider_FPSCap.SetValueWithoutNotify(Hypatios.Settings.MAXIMUM_FRAMERATE);
        slider_UIScaling.SetValueWithoutNotify(Hypatios.Settings.UI_SCALING);
        toggle_VSync.SetIsOnWithoutNotify(Hypatios.Settings.IntToBool(Hypatios.Settings.VSYNC));
        toggle_Fullscreen.SetIsOnWithoutNotify(Hypatios.Settings.IntToBool(Hypatios.Settings.FULLSCREEN));
        toggle_DynamicUIScaling.SetIsOnWithoutNotify(Hypatios.Settings.IntToBool(Hypatios.Settings.DYNAMIC_UI_SCALING));
        toggle_AutoDialogue.SetIsOnWithoutNotify(Hypatios.Settings.IntToBool(Hypatios.Settings.AUTO_DIALOGUE));
        toggle_MotionBlur.SetIsOnWithoutNotify(Hypatios.Settings.IntToBool(Hypatios.Settings.MOTIONBLUR));
        toggle_AntiAliasing.SetIsOnWithoutNotify(Hypatios.Settings.IntToBool(Hypatios.Settings.ANTIALIASING));
        toggle_UITVEffect.SetIsOnWithoutNotify(Hypatios.Settings.IntToBool(Hypatios.Settings.TV_EFFECT_UI));
        toggle_RunInBackground.SetIsOnWithoutNotify(Hypatios.Settings.IntToBool(Hypatios.Settings.RUN_IN_BACKGROUND));
        dropdown_Resolution.SetValueWithoutNotify(Hypatios.Settings.RESOLUTION);
        dropdown_Quality.SetValueWithoutNotify(Hypatios.Settings.QUALITY_LEVEL);
        //inputfield_Name.SetTextWithoutNotify(Hypatios.Settings.MY_NAME);

        RefreshUI();

    }


    public void Tutorial()
    {
        Application.LoadLevel(1);
    }


    public void SetSomething()
    {

        PlayerPrefs.Save();
        RefreshUI();

    }

    public void SetQualityLevel(int level)
    {
        Hypatios.Settings.QUALITY_LEVEL = level;
        RefreshUI();
    }



    private void RefreshUI()
    {

        //for (int x = 0; x < qualityLevels.Count; x++)
        //{
        //    var button = qualityLevels[x];

        //    if (x == Hypatios.Settings.QUALITY_LEVEL)
        //    {
        //        button.interactable = false;
        //    }
        //    else
        //    {
        //        button.interactable = true;
        //    }

        //}

        {
            //Hypatios.Settings.MY_NAME = inputfield_Name.text;
            Hypatios.Settings.MAXIMUM_FRAMERATE = Mathf.RoundToInt(slider_FPSCap.value);
            Hypatios.Settings.UI_SCALING = slider_UIScaling.value;
            Hypatios.Settings.RESOLUTION = Mathf.RoundToInt(dropdown_Resolution.value);
            Hypatios.Settings.QUALITY_LEVEL = Mathf.RoundToInt(dropdown_Quality.value);
            Hypatios.Settings.BRIGHTNESS = slider_Brightness.value;
            Hypatios.Settings.VISUAL_TEMPERATURE = Mathf.RoundToInt(slider_Temperature.value);
            Hypatios.Settings.VISUAL_TINT = Mathf.RoundToInt(slider_Tint.value);
            Hypatios.Settings.FOV = slider_FOV.value;
            Hypatios.Settings.MOTIONBLUR = toggle_MotionBlur.isOn ? 1 : 0;
            Hypatios.Settings.ANTIALIASING = toggle_AntiAliasing.isOn ? 1 : 0;
            Hypatios.Settings.TV_EFFECT_UI = toggle_UITVEffect.isOn ? 1 : 0;
            //Hypatios.Settings.DYNAMIC_UI_SCALING = toggle_DynamicUIScaling.isOn ? 1 : 0;      
            Hypatios.Settings.VSYNC = toggle_VSync.isOn ? 1 : 0;
            Hypatios.Settings.RUN_IN_BACKGROUND = toggle_RunInBackground.isOn ? 1 : 0;
            Hypatios.Settings.AUTO_DIALOGUE = toggle_AutoDialogue.isOn ? 1 : 0;
            Hypatios.Settings.FULLSCREEN = toggle_Fullscreen.isOn ? 1 : 0;
            Hypatios.Settings.MOUSE_SENSITIVITY = slider_MouseSensitivity.value;
            Hypatios.Settings.MUSIC_VOLUME = slider_Music.value;
            Hypatios.Settings.SFX_VOLUME = slider_SFX.value;
            PlayerPrefs.SetString("SETTINGS.MY_NAME", Hypatios.Settings.MY_NAME);
            PlayerPrefs.SetInt("SETTINGS.MAXIMUM_FRAMERATE", Hypatios.Settings.MAXIMUM_FRAMERATE);
            PlayerPrefs.SetFloat("SETTINGS.UI_SCALING", Hypatios.Settings.UI_SCALING);
            PlayerPrefs.SetInt("SETTINGS.RESOLUTION", Hypatios.Settings.RESOLUTION);
            PlayerPrefs.SetFloat("SETTINGS.MUSIC_VOLUME", Hypatios.Settings.MUSIC_VOLUME);
            PlayerPrefs.SetFloat("SETTINGS.SFX_VOLUME", Hypatios.Settings.SFX_VOLUME);
            PlayerPrefs.SetFloat("SETTINGS.BRIGHTNESS", Hypatios.Settings.BRIGHTNESS);
            PlayerPrefs.SetInt("SETTINGS.VISUAL_TEMPERATURE", Hypatios.Settings.VISUAL_TEMPERATURE);
            PlayerPrefs.SetInt("SETTINGS.VISUAL_TINT", Hypatios.Settings.VISUAL_TINT);
            PlayerPrefs.SetInt("SETTINGS.TV_EFFECT_UI", Hypatios.Settings.TV_EFFECT_UI);
            PlayerPrefs.SetInt("SETTINGS.RUN_IN_BACKGROUND", Hypatios.Settings.RUN_IN_BACKGROUND);
            PlayerPrefs.SetFloat("SETTINGS.FOV", Hypatios.Settings.FOV);
            PlayerPrefs.SetInt("SETTINGS.MOTIONBLUR", Hypatios.Settings.MOTIONBLUR);
            PlayerPrefs.SetInt("SETTINGS.ANTIALIASING", Hypatios.Settings.ANTIALIASING);
            //PlayerPrefs.SetInt("SETTINGS.DYNAMIC_UI_SCALING", Hypatios.Settings.DYNAMIC_UI_SCALING);
            PlayerPrefs.SetInt("SETTINGS.VSYNC", Hypatios.Settings.VSYNC);
            PlayerPrefs.SetInt("SETTINGS.FULLSCREEN", Hypatios.Settings.FULLSCREEN);
            PlayerPrefs.SetInt("SETTINGS.AUTO_DIALOGUE", Hypatios.Settings.AUTO_DIALOGUE);
            PlayerPrefs.SetFloat("SETTINGS.MOUSE_SENSITIVITY", Hypatios.Settings.MOUSE_SENSITIVITY);
            PlayerPrefs.SetInt("SETTINGS.QUALITY_LEVEL", Hypatios.Settings.QUALITY_LEVEL);

        }

        Debug.Log(Hypatios.Settings.SFX_VOLUME);


        float displayBrightness = (Hypatios.Settings.BRIGHTNESS + 1) / 2;

        value_SFX.text = Mathf.RoundToInt(Hypatios.Settings.SFX_VOLUME * 100).ToString();
        value_Music.text = Mathf.RoundToInt(Hypatios.Settings.MUSIC_VOLUME * 100).ToString();
        value_MouseSensitivity.text = (Mathf.Round(Hypatios.Settings.MOUSE_SENSITIVITY * 10)/10).ToString();
        value_FOV.text = (Mathf.Round(Hypatios.Settings.FOV)).ToString();
        value_Brightness.text = (Mathf.Round(displayBrightness * 10)/10).ToString();
        value_UIScaling.text = $"x{(Mathf.Round((1.5f - Hypatios.Settings.UI_SCALING) * 10) / 10)}";
        value_Temperature.text = (Hypatios.Settings.VISUAL_TEMPERATURE).ToString();
        value_Tint.text = (Hypatios.Settings.VISUAL_TINT).ToString();

        if (Hypatios.Settings.MAXIMUM_FRAMERATE < 201)
        value_FPSCap.text = (Mathf.Round(Hypatios.Settings.MAXIMUM_FRAMERATE)).ToString();
        else value_FPSCap.text = "∞".ToString();

        Hypatios.Settings1.RefreshSettings();
        PlayerPrefs.Save();


        //colorGrading.gamma.value.x = 1 - (BRIGHTNESS / 3f);
        //colorGrading.gamma.value.y = 1 - (BRIGHTNESS / 3f);
        //colorGrading.gamma.value.z = 1 - (BRIGHTNESS / 3f);



    }

}
