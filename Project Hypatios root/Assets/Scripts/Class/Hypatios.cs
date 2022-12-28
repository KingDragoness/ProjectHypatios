using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;
using Sirenix.OdinInspector;
using System.Linq;


public class Hypatios : MonoBehaviour
{

    [System.Serializable]
    public class Settings
    {
        //Manual references
        [FoldoutGroup("Sounds")] public AudioMixer mixerSFX;
        [FoldoutGroup("Sounds")] public AudioMixer mixerMusic;

        public static int QUALITY_LEVEL = 0;
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
        private static Resolution[] resolutions;

        public static Resolution[] Resolutions { get => resolutions; }

        internal void InitializeAtAwake()
        {
            var validRes = Screen.resolutions;
            resolutions = validRes;
            AO = Game.postProcessVolume.profile.GetSetting<AmbientOcclusion>();
            motionBlur = Game.postProcessVolume.profile.GetSetting<MotionBlur>();
            realtimeReflections = Game.postProcessVolume.profile.GetSetting<ScreenSpaceReflections>();
            colorGrading = Game.postProcessVolume_2.profile.GetSetting<ColorGrading>();

            MY_NAME = LoadPrefKeyString("SETTINGS.MY_NAME", "Aldrich");
            BRIGHTNESS = LoadPrefKeyFloat("SETTINGS.BRIGHTNESS", 0.5f);
            MAXIMUM_FRAMERATE = LoadPrefKeyInt("SETTINGS.MAXIMUM_FRAMERATE", 201);
            RESOLUTION = LoadPrefKeyInt("SETTINGS.RESOLUTION", resolutions.Length - 1);
            MOUSE_SENSITIVITY = LoadPrefKeyFloat("SETTINGS.MOUSE_SENSITIVITY", 10f);
            MOTIONBLUR = LoadPrefKeyInt("SETTINGS.MOTIONBLUR", 1);
            SFX_VOLUME = AssignValuePref("SETTINGS.SFX_VOLUME", 1); 
            MUSIC_VOLUME = AssignValuePref("SETTINGS.MUSIC_VOLUME", 1);
            QUALITY_LEVEL = LoadPrefKeyInt("SETTINGS.QUALITY_LEVEL", 1); 
            VSYNC = PlayerPrefs.GetInt("SETTINGS.VSYNC");

            {

                floatParam_Brightness = colorGrading.postExposure;
            }

            RefreshSettings();
        }

        internal void InitializeAtStart()
        {
            QualitySettings.SetQualityLevel(QUALITY_LEVEL);
            RefreshSettings();
        }

        public void RefreshSettings()
        {

            if (MAXIMUM_FRAMERATE >= 201)
                Application.targetFrameRate = -1; else Application.targetFrameRate = MAXIMUM_FRAMERATE;

            if (RESOLUTION != -1)
                Screen.SetResolution(resolutions[RESOLUTION].width, resolutions[RESOLUTION].height, Screen.fullScreen);

            if (VSYNC == 0)
                QualitySettings.vSyncCount = 0; else QualitySettings.vSyncCount = 1;

            if (colorGrading != null)
            { colorGrading.gamma.value.w = (BRIGHTNESS / 2f); floatParam_Brightness.value = (BRIGHTNESS / 2f); }

            if (MainCameraScript != null)
                MainCameraScript.mouseSensitivity = MOUSE_SENSITIVITY;

            mixerSFX.SetFloat("Master", Mathf.Log10(SFX_VOLUME) * 20);
            mixerMusic.SetFloat("Master", Mathf.Log10(MUSIC_VOLUME) * 20);

            if (realtimeReflections != null)
            {
                if (QUALITY_LEVEL != 2)
                    realtimeReflections.active = false;
                else
                    realtimeReflections.active = true;
            }

            if (AO != null)
            {
                if (QUALITY_LEVEL == 0)
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
                    if (Game.minorMotionBlur != null) Game.minorMotionBlur.enabled = true;
                }
                else
                {
                    motionBlur.active = true;
                    if (Game.minorMotionBlur != null) Game.minorMotionBlur.enabled = false;
                }
            }

        }

        #region KeyPrefs
        public static string LoadPrefKeyString(string Keyname, string defaultValue)
        {
            if (PlayerPrefs.HasKey(Keyname))
                return PlayerPrefs.GetString(Keyname);
            else
                return defaultValue;
        }
        public static float LoadPrefKeyFloat(string Keyname, float defaultValue)
        {
            if (PlayerPrefs.HasKey(Keyname))
                return PlayerPrefs.GetFloat(Keyname);
            else
                return defaultValue;
        }

        public static int LoadPrefKeyInt(string Keyname, int defaultValue)
        {
            if (PlayerPrefs.HasKey(Keyname))
                return PlayerPrefs.GetInt(Keyname);
            else
                return defaultValue;
        }

        public static float AssignValuePref(string KeyName, float defaultValue)
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

        public static bool IntToBool(int a)
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
    }


    public enum GameDifficulty
    {
        Peaceful = 0, //AI deals absolutely no damage or not shooting
        Casual = 1, //dumbed down enemy AI, optimized for console player
        Normal = 10, //for casual first-person
        Hard = 20, //for those who has experience in FPS (default difficulty)
        Brutal = 30, //for FPS expert, enemy can deal very high damage & chance of crits
    }

    //For console casuals
    public static float ExtraAttackSpeedModifier()
    {
        if (Instance._gameDifficulty == GameDifficulty.Casual)
        {
            return 1.6f;
        }
        else
        {
            return 1;
        }
    }

    #region Systems
    [SerializeField] private GameDifficulty _gameDifficulty = GameDifficulty.Hard;
    public static GameDifficulty Difficulty { get => Instance._gameDifficulty; set => Instance._gameDifficulty = value; }

    #endregion

    [SerializeField]
    private HypatiosControls _actionMap;

    [SerializeField]
    private FPSMainScript _fpsMainScript;

    [SerializeField]
    private CharacterScript _characterScript;

    [SerializeField]
    private AssetStorageDatabase _assetStorage;

    [SerializeField]
    private MainUI _ui;

    [SerializeField]
    private Camera _mainCamera;

    [SerializeField]
    private cameraScript _mainCameraScript;

    [SerializeField]
    private DynamicObjectPool _dynamicObjectPool;

    [SerializeField]
    private EnemyContainer _enemyContainer;

    [SerializeField]
    private Debug_ObjectStat _debugObjectStat;

    [SerializeField]
    private Settings _settings;

    public static FPSMainScript Game { get => Instance._fpsMainScript; }
    public static CharacterScript Player { get => Instance._characterScript; }
    public static AssetStorageDatabase Assets { get => Instance._assetStorage; }
    public static MainUI UI { get => Instance._ui; }
    public static Camera MainCamera { get => Instance._mainCamera; }
    public static cameraScript MainCameraScript { get => Instance._mainCameraScript; }
    public static DynamicObjectPool ObjectPool { get => Instance._dynamicObjectPool; }
    public static EnemyContainer Enemy { get => Instance._enemyContainer; }
    public static HypatiosControls.DefaultActions Input { get => Instance._actionMap.Default; }
    public static Debug_ObjectStat DebugObjectStat { get => Instance._debugObjectStat; }
    public static Settings Settings1 { get => Instance._settings; set => Instance._settings = value; }

    public static float Time { get => Game.UNIX_Timespan; }

    public static Hypatios Instance;

    private void Awake()
    {
        Instance = this;
        _actionMap = new HypatiosControls();
        _actionMap.Enable();
        Settings1.InitializeAtAwake();
        HackFixForEditorPlayModeDelay();
    }

    //DA FUCK
    //https://forum.unity.com/threads/input-freezes-for-several-seconds-shortly-after-repeatedly-entering-play-mode.838333/page-2
    void HackFixForEditorPlayModeDelay()
    {
    #if UNITY_EDITOR
            // Using reflection, does this: InputSystem.s_SystemObject.exitEditModeTime = 0

            // Get InputSystem.s_SystemObject object
            FieldInfo systemObjectField = typeof(UnityEngine.InputSystem.InputSystem).GetField("s_SystemObject", BindingFlags.NonPublic | BindingFlags.Static);
            object systemObject = systemObjectField.GetValue(null);

            // Get InputSystemObject.exitEditModeTime field
            Assembly inputSystemAssembly = typeof(UnityEngine.InputSystem.InputSystem).Assembly;
            System.Type inputSystemObjectType = inputSystemAssembly.GetType("UnityEngine.InputSystem.InputSystemObject");
            FieldInfo exitEditModeTimeField = inputSystemObjectType.GetField("exitEditModeTime", BindingFlags.Public | BindingFlags.Instance);

            // Set exitEditModeTime to zero
            exitEditModeTimeField.SetValue(systemObject, 0d);
    #endif
    }

    private void Start()
    {
        Settings1.InitializeAtStart();
    }

    public static int GetSeed()
    {
        int i1 = 0;

        if (SystemInfo.deviceName != null)
        {
            char f1 = SystemInfo.deviceName[0];
            i1 = f1 - '0';
        }

        int seed = Hypatios.Game.TotalRuns + SystemInfo.graphicsDeviceID + SystemInfo.graphicsDeviceVendorID + i1 + SystemInfo.graphicsMemorySize;

        return seed;
    }

    public static float GetRandomChance()
    {
        int seed = GetSeed();
        var RandomSys = new System.Random(seed);
        float random = (RandomSys.Next(0, 100)) / 100f;

        return random;
    }

}
