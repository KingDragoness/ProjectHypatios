using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;
using Gyroscope = UnityEngine.InputSystem.Gyroscope;

public class MainUI : MonoBehaviour
{

    public enum UIMode
    {
        Default,
        Weapon,
        Paradox,
        Crafting,
        Shop,
        Cinematic,
        FreecamMode
    }

    [FoldoutGroup("References")] public GameObject PauseMenu;
    [FoldoutGroup("References")] public GameObject HUD;
    [FoldoutGroup("References")] public GameObject Shop_Weapon_UI;
    [FoldoutGroup("References")] public GameObject Shop_Paradox_UI;
    [FoldoutGroup("References")] public GameObject CutsceneHUD_UI;
    [FoldoutGroup("References")] public GameObject CraftingWeapon_UI;
    [FoldoutGroup("References")] public GameObject DefaultHUD_UI;
    [FoldoutGroup("References")] public GameObject Console_UI;
    [FoldoutGroup("References")] public GameObject Camera_Cutscene;
    [FoldoutGroup("References")] public GameObject Camera_Main;
    [FoldoutGroup("References")] public GameObject SavingGameIcon_UI;
    [FoldoutGroup("References")] public CutsceneDialogueUI cutsceneUI;
    [FoldoutGroup("References")] public SettingsUI settingsUI;
    [FoldoutGroup("References")] public NoclipCamera Camera_Noclip;
    [FoldoutGroup("References")] public GameObject Player;
    [FoldoutGroup("References")] public SpawnIndicator SpawnIndicator;
    public UIMode current_UI = UIMode.Default;
    private bool paused = false;

    public static MainUI Instance;

    private CanvasGroup cg_Shop;
    private CanvasGroup cg_defaultHUD;
    private CanvasGroup cg_Paradox;

    private bool tempoPause = false;

    private void Awake()
    {
        Instance = this;

        cg_Shop = Shop_Weapon_UI.GetComponent<CanvasGroup>();
        cg_defaultHUD = DefaultHUD_UI.GetComponent<CanvasGroup>();
        cg_Paradox = Shop_Paradox_UI.GetComponent<CanvasGroup>();

    }

    private void Start()
    {
        Screen.fullScreen = true;

    }

    public static float CalculateRatioX()
    {
        float x = Screen.width;
        x /= 900f;
        return x;
    }

    public static float CalculateRatioY()
    {
        float y = Screen.height;
        y /= 900f;
        return y;
    }

    public void GamePause(bool doPause = false)
    {
        paused = !paused;
        RefreshPauseState();

    }

    public void ChangeCurrentMode(int i)
    {
        current_UI = (MainUI.UIMode)i;

    }

    public void Unpause()
    {
        Time.timeScale = 1;
    }

    private bool b_OnActivateNoClipMode = false;

    void Update()
    {
        if (Hypatios.Input.Pause.triggered)
        {
            bool allowToggle = false;

            if ((current_UI == UIMode.Default | current_UI == UIMode.Cinematic | current_UI == UIMode.FreecamMode)
                && Hypatios.Player.Health.isDead == false)
            {
                allowToggle = true;
            }
            else
            {
                if (!paused)
                {
                    current_UI = UIMode.Default;
                }
                else
                {
                    allowToggle = true;
                }
            }

            if (allowToggle)
            {
                paused = !paused;
                tempoPause = false;
                RefreshPauseState();
            }
        }

        if (Input.GetKeyUp(KeyCode.BackQuote))
        {
            Console_UI.gameObject.SetActive(!Console_UI.gameObject.activeSelf);
        }

        if (Input.GetKeyUp(KeyCode.F11))
        {
            Screen.fullScreen = !Screen.fullScreen;
        }

        if (!paused)
        {

            bool isIdlePlayer = false;

            if (current_UI == UIMode.Default)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Shop_Weapon_UI.gameObject.SetActive(false);
                Shop_Paradox_UI.gameObject.SetActive(false);
                CraftingWeapon_UI.gameObject.SetActive(false);
                CutsceneHUD_UI.gameObject.SetActive(false);
                DefaultHUD_UI.gameObject.SetActive(true);

                if (Hypatios.Player.Health.isDead == false)
                {
                    Time.timeScale = 1;
                }
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                DefaultHUD_UI.gameObject.SetActive(false);
            }


            if (current_UI == UIMode.Crafting | current_UI == UIMode.Paradox | current_UI == UIMode.Weapon | current_UI == UIMode.Shop | current_UI == UIMode.Cinematic
                | current_UI == UIMode.FreecamMode)
            {
                isIdlePlayer = true;
            }

            if (isIdlePlayer)
            {
                Camera_Main.gameObject.SetActive(false);
                Camera_Cutscene.gameObject.SetActive(true);

                Hypatios.Player.disableInput = true;
                Hypatios.Player.enabled = false;
                Hypatios.Player.rb.isKinematic = true;
                Hypatios.Player.Health.enabled = false;
                soundManagerScript.instance.Pause("running");
            }
            else if (Hypatios.Player.Health.isDead == false)
            {
                Camera_Main.gameObject.SetActive(true);
                Camera_Cutscene.gameObject.SetActive(false);
                Hypatios.Player.disableInput = false;
                Hypatios.Player.enabled = true;
                Hypatios.Player.rb.isKinematic = false;
                Hypatios.Player.Health.enabled = true;
            }

            //custom modes
            {
                if (current_UI == UIMode.FreecamMode)
                {
                    Camera_Noclip.gameObject.SetActive(true);


                    if (b_OnActivateNoClipMode == false)
                    {
                        Camera_Noclip.transform.position = Player.transform.position;
                    }

                    b_OnActivateNoClipMode = true;
                }
                else
                {
                    Camera_Noclip.gameObject.SetActive(false);
                    b_OnActivateNoClipMode = false;
                }

                if (current_UI == UIMode.Paradox)
                {
                    Shop_Paradox_UI.gameObject.SetActive(true);

                    if (ParadoxShopOwner.Instance != null)
                        ParadoxShopOwner.Instance.EnableStateParadox();
                }
                else
                {
                    if (ParadoxShopOwner.Instance != null)
                        ParadoxShopOwner.Instance.DisableStateParadox();
                }

                if (current_UI == UIMode.Weapon)
                {
                    Shop_Weapon_UI.gameObject.SetActive(true);
                }

                if (current_UI == UIMode.Crafting)
                {
                    CraftingWeapon_UI.gameObject.SetActive(true);
                }

                if (current_UI == UIMode.Cinematic)
                {
                    CutsceneHUD_UI.gameObject.SetActive(true);
                }
            }

            if (tempoPause)
            {
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
            }
        }
    }

    public void SetTempoPause(bool pause)
    {
        tempoPause = pause;
    }

    private void RefreshPauseState()
    {
        if (paused == false)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            PauseMenu.gameObject.SetActive(false);
            Time.timeScale = 1;
            //Player.gameObject.SetActive(true);
            HUD.gameObject.SetActive(true);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            PauseMenu.gameObject.SetActive(true);
            Time.timeScale = 0;

            //Player.gameObject.SetActive(false);
            HUD.gameObject.SetActive(false);


        }

        if (tempoPause == false)
        {
            MainGameHUDScript.Instance.promptUIMain.SetActive(false);
        }

        if (Gamepad.all.Count > 0)
        {
            Debug.Log("Gamepad detected, switching difficulty to Casual.");
            Hypatios.Difficulty = Hypatios.GameDifficulty.Casual;


            //cancelled because no gyro in Xbox One controller
            if (UnityEngine.InputSystem.Gyroscope.current != null)
            {

                Debug.Log("Gyroscope detected, enabling gyroscope.");
                InputSystem.EnableDevice(UnityEngine.InputSystem.Gyroscope.current);
            }
        }
        else
        {
            if (Hypatios.Difficulty == Hypatios.GameDifficulty.Casual)
            {
                Debug.Log("Switching difficulty back to normal.");
                Hypatios.Difficulty = Hypatios.GameDifficulty.Normal;
            }
        }
    }

    public void Demo_PlayLevel1()
    {
        Application.LoadLevel(1);
    }

    public void Demo_PlayLevel(int level)
    {
        Application.LoadLevel(level);
    }

    public void Demo_Restart()
    {
        Time.timeScale = 1;

        if (Hypatios.Game.currentGamemode != FPSMainScript.CurrentGamemode.TutorialMode
            && Hypatios.Game.currentGamemode != FPSMainScript.CurrentGamemode.Elena)
        {
            //Hypatios.Game.SaveGame(targetLevel: Application.loadedLevel);
            if (FPSMainScript.CheckSaveFileExist()) Hypatios.Game.BufferSaveData();
        }

        Application.LoadLevel(Application.loadedLevel);
    }

    public void Demo_QuitToMenu()
    {
        Time.timeScale = 1;
        Application.LoadLevel(0);
    }

    public void Demo_QuitGame()
    {
        Debug.Log("GAME QUIT");
        Application.Quit();
    }
}
