using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;
using UnityEngine.UI.Extensions;
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
        Cinematic, //please note if bool "is_CachedCinematicMode" is not checked then it will fallback to Default
        FreecamMode,
        Trivia,
        Favorite,
        kThanidLab,
        MobiusNet,
        LevelMap,
        MachineOfMadness,
        HypatiosMap //hypatios chamber map (sewers, intermezzo, all chambers; no WIRED level)
    }

    public bool disableInput = false;
    public MobiusNetUI Internet;
    public ReadableBookUI readableBookUI;

    [FoldoutGroup("References")] public GameObject PauseMenu;
    [FoldoutGroup("References")] public GameObject HUD;
    [FoldoutGroup("References")] public CanvasScaler scaler_Main;
    [FoldoutGroup("References")] public CanvasScaler scaler_Pause;
    [FoldoutGroup("References")] public CanvasScaler scaler_Trivia;
    [FoldoutGroup("References")] public Canvas canvas_Main;
    [FoldoutGroup("References")] public PauseRestartGameButton pauseGameSystem;
    [FoldoutGroup("References")] public MainGameHUDScript mainHUDScript;
    [FoldoutGroup("References")] public GameObject Shop_Weapon_UI;
    [FoldoutGroup("References")] public GameObject Shop_Paradox_UI;
    [FoldoutGroup("References")] public GameObject CutsceneHUD_UI;
    [FoldoutGroup("References")] public GameObject CraftingWeapon_UI;
    [FoldoutGroup("References")] public GameObject kThanidLab_UI;
    [FoldoutGroup("References")] public GameObject DefaultHUD_UI;
    [FoldoutGroup("References")] public GameObject FavoriteHUD_UI;
    [FoldoutGroup("References")] public GameObject Console_UI;
    [FoldoutGroup("References")] public GameObject Camera_Cutscene;
    [FoldoutGroup("References")] public GameObject Camera_Main;
    [FoldoutGroup("References")] public NoclipCamera Camera_Noclip;
    [FoldoutGroup("References")] public PromptMessengerUI PromptMessage_UI;
    [FoldoutGroup("References")] public AchievementNotifyUI AchievementNotify_UI;
    [FoldoutGroup("References")] public GameObject SavingGameIcon_UI;
    [FoldoutGroup("References")] public CutsceneDialogueUI cutsceneUI;
    [FoldoutGroup("References")] public SettingsUI settingsUI;
    [FoldoutGroup("References")] public GameObject TriviaMap;
    [FoldoutGroup("References")] public GameObject LevelMap;
    [FoldoutGroup("References")] public MachineOfMadnessUI MachineOfMadnessMap;
    [FoldoutGroup("References")] public GameObject Player;
    [FoldoutGroup("References")] public SpawnIndicator SpawnIndicator;
    [FoldoutGroup("References")] public ContextMenuUI ContextMenuUI;
    [FoldoutGroup("References")] public WindowConfirmPrompt ConfirmPromptUI;
    [FoldoutGroup("References")] public GameObject postProcessUI;

    [FoldoutGroup("Tooltips")] public TestingPurposes.UIElementScreenPosTest screenPosChecker;
    [FoldoutGroup("Tooltips")] public RectTransform tooltipBig_rect;
    [FoldoutGroup("Tooltips")] public RectTransform tooltipSmall_rect;
    [FoldoutGroup("Tooltips")] public RectTransform testingTooltipPos;
    [FoldoutGroup("Tooltips")] public ToolTip TooltipBig;
    [FoldoutGroup("Tooltips")] public ToolTip TooltipSmall;
    [FoldoutGroup("Tooltips")] public Text bigTooltip_LeftHandedLabel;
    [FoldoutGroup("Tooltips")] public Text bigTooltip_RightHandedLabel;
    [FoldoutGroup("Tooltips")] public Text smallTooltip_LeftHandedLabel;
    [FoldoutGroup("Tooltips")] public Text smallTooltip_RightHandedLabel;
    [FoldoutGroup("Tooltips")] public GameObject tooltip_IconInventory;
    [FoldoutGroup("Tooltips")] public Image tooltip_IconInventoryImage;

    [FoldoutGroup("Events")] public GameEvent event_GamePause;
    [FoldoutGroup("Events")] public GameEvent event_GameResume;

    public UIMode CurrentUI { get => current_UI; set => current_UI = value; }

    [Range(0f, 1f)] public float UI_Scaling = 1f;
    private bool paused = false;

    public static MainUI Instance;

    private CanvasGroup cg_Shop;
    private CanvasGroup cg_defaultHUD;
    private CanvasGroup cg_Paradox;
    private UIMode current_UI = UIMode.Default;

    private bool tempoPause = false;
    private bool is_CachedCinematicMode = false;

    private void Awake()
    {
        Instance = this;

        cg_Shop = Shop_Weapon_UI.GetComponent<CanvasGroup>();
        cg_defaultHUD = DefaultHUD_UI.GetComponent<CanvasGroup>();
        cg_Paradox = Shop_Paradox_UI.GetComponent<CanvasGroup>();

    }

    private void Start()
    {

    }

    #region Utility
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

    public void SetPauseState(bool pauseState = false)
    {
        paused = pauseState;
        RefreshPauseState();

    }

    public void ChangeCurrentMode(int i)
    {
        CurrentUI = (MainUI.UIMode)i;

    }
    public void ChangeCurrentMode(MainUI.UIMode mode)
    {
        CurrentUI = mode;

    }

    public void Unpause()
    {
        Time.timeScale = 1;
    }



    #endregion


    #region TOOLTIPS

    public void ShowTooltipBig(RectTransform currentSelection, string leftText = "$%", string rightText = "$%")
    {
        tooltipBig_rect.gameObject.SetActive(true);
        var v3 = screenPosChecker.GetPositionScreenForTooltip(currentSelection);
        tooltipBig_rect.position = v3;
        testingTooltipPos.position = v3;

        if (leftText != "$%")
        {
            bigTooltip_LeftHandedLabel.text = leftText;
        }

        if (rightText != "$%")
        {
            bigTooltip_RightHandedLabel.text = rightText;

        }
    }

    public void ShowContextMenu(Vector2 positionMouse, List<ContextMenuUI.ContextCommandElement> commands)
    {
        ContextMenuUI.ShowContextMenu(positionMouse, commands);

    }
    public void PromptNotifyMessage(string text, float time)
    {
        DeadDialogue.PromptNotifyMessage_Mod(text, time);
    }


    public void ShowTooltipSmall(RectTransform currentSelection)
    {
        tooltipSmall_rect.gameObject.SetActive(true);
        var v3 = screenPosChecker.GetPositionScreenForTooltip(currentSelection);
        tooltipSmall_rect.position = v3;
        testingTooltipPos.position = v3;
    }


    public void RefreshInventoryIcon(Sprite sprite, Color? _color = null)
    {
        if (sprite == null)
        {
            tooltip_IconInventory.gameObject.SetActive(false);
            return;
        }

        tooltip_IconInventory.gameObject.SetActive(true);
        tooltip_IconInventoryImage.sprite = sprite;
        tooltip_IconInventoryImage.color = _color ?? Color.white;

    }


    public void ShowTooltipSmall(RectTransform currentSelection, string str_left, string str_right)
    {
        tooltipSmall_rect.gameObject.SetActive(true);
        var v3 = screenPosChecker.GetPositionScreenForTooltip(currentSelection);
        tooltipSmall_rect.position = v3;
        testingTooltipPos.position = v3;
        smallTooltip_LeftHandedLabel.text = str_left;
        smallTooltip_RightHandedLabel.text = str_right;
    }

    public void CloseAllTooltip()
    {
        tooltipBig_rect.gameObject.SetActive(false);
        tooltipSmall_rect.gameObject.SetActive(false);

    }
    #endregion

    private bool b_OnActivateNoClipMode = false;


    void Update()
    {
        //Escape when Trivia Map mode = go back to main menu
        if (disableInput == false)
        {
            if (Hypatios.Input.Pause.triggered | Input.GetKeyUp(KeyCode.Tab))
            {
                bool allowToggle = false;


                if (Input.GetKeyUp(KeyCode.Tab))
                {
                    pauseGameSystem.SetMenuType(2);
                }

                if ((CurrentUI == UIMode.Default | CurrentUI == UIMode.Cinematic | CurrentUI == UIMode.FreecamMode | CurrentUI == UIMode.Trivia | CurrentUI == UIMode.LevelMap | current_UI == UIMode.MachineOfMadness)
                    && Hypatios.Player.Health.isDead == false)
                {
                    allowToggle = true;
                }
                else if (CurrentUI != UIMode.Favorite)
                {
                    if (!paused)
                    {
                        CurrentUI = UIMode.Default;
                    }
                    else
                    {
                        allowToggle = true;
                    }
                }

                if (CurrentUI != UIMode.LevelMap && CurrentUI != UIMode.Trivia && CurrentUI != UIMode.Favorite && CurrentUI != UIMode.MachineOfMadness)
                {
                    if (allowToggle)
                    {
                        paused = !paused;
                        tempoPause = false;
                        RefreshPauseState();
                    }
                }
                else if (CurrentUI == UIMode.Trivia | CurrentUI == UIMode.LevelMap | CurrentUI == UIMode.MachineOfMadness)
                {
                    if (allowToggle)
                    {
                        //back to main menu or reset whatever last set
                        CurrentUI = UIMode.Default;
                    }
                }

            }



                //Favorite menu is now removed from the game.
                {
                //if (CurrentUI == UIMode.Default && Hypatios.Player.Health.isDead == false && paused == false)
                //{
                //    if (Input.GetKeyUp(KeyCode.Tab))
                //    {
                //        CurrentUI = UIMode.Favorite;
                //        SetTempoPause(true);
                //    }
                //}
                //else if (CurrentUI == UIMode.Favorite && (Input.GetKeyUp(KeyCode.Tab) | Hypatios.Input.Pause.triggered))
                //{
                //    CurrentUI = UIMode.Default;
                //    SetTempoPause(false);
                //}
            }
        }

        if (Input.GetKeyUp(KeyCode.BackQuote) && Hypatios.IsDemoMode == false)
        {
            Console_UI.gameObject.SetActive(!Console_UI.gameObject.activeSelf);
        }

        if (Input.GetKeyUp(KeyCode.F11))
        {
            Screen.fullScreen = !Screen.fullScreen;
        }

        if (Input.GetKeyUp(KeyCode.F3))
        {
            bool b = Hypatios.DebugObjectStat.gameObject.activeSelf;
            Hypatios.DebugObjectStat.gameObject.SetActive(!b);
        }



        RefreshUI_Resolutions();


        if (!paused)
        {

            bool isIdlePlayer = false;
            bool isUsingCustomCam = false;
            bool enable_PostFXUI = false;

            if (CurrentUI == UIMode.Default)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Shop_Weapon_UI.gameObject.SetActive(false);
                Shop_Paradox_UI.gameObject.SetActive(false);
                CraftingWeapon_UI.gameObject.SetActive(false);
                kThanidLab_UI.gameObject.SetActive(false);
                TriviaMap.gameObject.SetActive(false);
                CutsceneHUD_UI.gameObject.SetActive(false);
                FavoriteHUD_UI.gameObject.SetActive(false);
                Internet.gameObject.SetActive(false);
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

            //Cutscene mode
            {
                if (CurrentUI == UIMode.Cinematic && is_CachedCinematicMode == false)
                {
                    CurrentUI = UIMode.Default;
                    ConsoleCommand.Instance.SendConsoleMessage("Cache cinematic mode safety bool is false. Reverting back to default.");
                }

                if (is_CachedCinematicMode == true)
                {
                    CurrentUI = UIMode.Cinematic;
                }
            }

            if (CurrentUI == UIMode.Crafting | CurrentUI == UIMode.kThanidLab | CurrentUI == UIMode.Paradox | CurrentUI == UIMode.Weapon | CurrentUI == UIMode.Shop 
                | CurrentUI == UIMode.FreecamMode | CurrentUI == UIMode.MobiusNet)
            {
                isIdlePlayer = true;
            }

            if (CurrentUI == UIMode.FreecamMode | CurrentUI == UIMode.Trivia | CurrentUI == UIMode.LevelMap | CurrentUI == UIMode.MachineOfMadness)
            {
                isUsingCustomCam = true;
            }

            if (isIdlePlayer)
            {
                Camera_Main.gameObject.SetActive(false);
                if (isUsingCustomCam == false) Camera_Cutscene.gameObject.SetActive(true); else Camera_Cutscene.gameObject.SetActive(false);


                Hypatios.Player.disableInput = true;
                Hypatios.Player.enabled = false;
                Hypatios.Player.rb.isKinematic = true;
                Hypatios.Player.Health.enabled = false;
                soundManagerScript.instance.Pause("running");
            }
            else if (current_UI == UIMode.Cinematic)
            {
                Camera_Main.gameObject.SetActive(true);
                Camera_Cutscene.gameObject.SetActive(false); //FUCK CUTSCENE
                Hypatios.Player.disableInput = true;
                Hypatios.Player.disableInput_CameraAllowed = true;
                Hypatios.Player.enabled = false;
                if (Hypatios.Player.isVehicleMode == false) Hypatios.Player.rb.isKinematic = false; else Hypatios.Player.rb.isKinematic = true;
                Hypatios.Player.Weapon.gameObject.EnableGameobject(false);
                soundManagerScript.instance.Pause("running");

            }
            else if (Hypatios.Player.Health.isDead == false)
            {
                Camera_Main.gameObject.SetActive(true);
                Camera_Cutscene.gameObject.SetActive(false);
                Hypatios.Player.disableInput = false;
                Hypatios.Player.disableInput_CameraAllowed = false;
                Hypatios.Player.enabled = true;
                if (Hypatios.Player.isVehicleMode == false) Hypatios.Player.rb.isKinematic = false; else Hypatios.Player.rb.isKinematic = true; //THIS IS FUCKING STUPID
                Hypatios.Player.Weapon.gameObject.EnableGameobject(true);
                Hypatios.Player.Health.enabled = true;
            }

            //custom modes
            {
                if (CurrentUI == UIMode.FreecamMode)
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

                if (CurrentUI == UIMode.Paradox)
                {
                    Shop_Paradox_UI.gameObject.SetActive(true);
                    enable_PostFXUI = true;

                    if (ParadoxShopOwner.Instance != null)
                        ParadoxShopOwner.Instance.EnableStateParadox();
                }
                else
                {
                    if (ParadoxShopOwner.Instance != null)
                        ParadoxShopOwner.Instance.DisableStateParadox();
                }

                if (CurrentUI == UIMode.Weapon)
                {
                    Shop_Weapon_UI.gameObject.SetActive(true);
                    enable_PostFXUI = true;
                }

                if (CurrentUI == UIMode.Crafting)
                {
                    CraftingWeapon_UI.gameObject.SetActive(true);
                    enable_PostFXUI = true;
                }

                if (CurrentUI == UIMode.kThanidLab)
                {
                    kThanidLab_UI.gameObject.SetActive(true);
                    enable_PostFXUI = true;
                }

                if (CurrentUI == UIMode.MobiusNet)
                {
                    Internet.gameObject.SetActive(true);
                    enable_PostFXUI = true;
                }

                if (CurrentUI == UIMode.Cinematic)
                {
                    CutsceneHUD_UI.gameObject.SetActive(true);
                    enable_PostFXUI = true;
                }

                if (CurrentUI == UIMode.Favorite)
                {
                    FavoriteHUD_UI.gameObject.SetActive(true);
                    Hypatios.Player.disableInput = true;
                    enable_PostFXUI = true;
                }

                if (CurrentUI == UIMode.MachineOfMadness)
                {
                    paused = true;
                    RefreshPauseState();
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

            if (enable_PostFXUI)
            {
                postProcessUI.gameObject.SetActive(true);
            }
            else
            {
                postProcessUI.gameObject.SetActive(false);
            }
        }

        if (paused)
        {
            postProcessUI.gameObject.SetActive(true);

            if (Hypatios.Input.Pause.triggered)
            {
                bool allowToggle = false;

                if ((CurrentUI == UIMode.Trivia | CurrentUI == UIMode.LevelMap | CurrentUI == UIMode.MachineOfMadness) && Hypatios.Player.Health.isDead == false)
                {
                    allowToggle = true;
                }        

                if (CurrentUI == UIMode.Trivia | CurrentUI == UIMode.LevelMap | CurrentUI == UIMode.MachineOfMadness)
                {
                    if (allowToggle)
                    {
                        CurrentUI = UIMode.Default;
                    }
                }

            }

            if (CurrentUI == UIMode.Trivia)
            {
                if (Camera_Main.activeSelf == true) Camera_Main.gameObject.SetActive(false);
                if (Camera_Noclip.gameObject.activeSelf == true) Camera_Noclip.gameObject.SetActive(false);
                if (TriviaMap.activeSelf == false) TriviaMap.gameObject.SetActive(true);
                if (LevelMap.activeSelf == true) LevelMap.gameObject.SetActive(false);
                if (PauseMenu.activeSelf == true) PauseMenu.gameObject.SetActive(false);
                if (MachineOfMadnessMap.gameObject.activeSelf == true) MachineOfMadnessMap.gameObject.SetActive(false);
            }
            else if (CurrentUI == UIMode.LevelMap)
            {
                if (Camera_Main.activeSelf == true) Camera_Main.gameObject.SetActive(false);
                if (Camera_Noclip.gameObject.activeSelf == true) Camera_Noclip.gameObject.SetActive(false);
                if (TriviaMap.activeSelf == true) TriviaMap.gameObject.SetActive(false);
                if (LevelMap.activeSelf == false) LevelMap.gameObject.SetActive(true);
                if (PauseMenu.activeSelf == true) PauseMenu.gameObject.SetActive(false);
                if (MachineOfMadnessMap.gameObject.activeSelf == true) MachineOfMadnessMap.gameObject.SetActive(false);

            }
            else if (CurrentUI == UIMode.MachineOfMadness)
            {
                if (Camera_Main.activeSelf == true) Camera_Main.gameObject.SetActive(false);
                if (Camera_Noclip.gameObject.activeSelf == true) Camera_Noclip.gameObject.SetActive(false);
                if (TriviaMap.activeSelf == true) TriviaMap.gameObject.SetActive(false);
                if (LevelMap.activeSelf == true) LevelMap.gameObject.SetActive(false);
                if (PauseMenu.activeSelf == true) PauseMenu.gameObject.SetActive(false);
                if (MachineOfMadnessMap.gameObject.activeSelf == false) MachineOfMadnessMap.gameObject.SetActive(true);

            }
            else
            {
                //COPY to else (!pause)
                if (Camera_Main.activeSelf == false) Camera_Main.gameObject.SetActive(true);
                if (TriviaMap.activeSelf == true) TriviaMap.gameObject.SetActive(false);
                if (LevelMap.activeSelf == true) LevelMap.gameObject.SetActive(false);
                if (PauseMenu.activeSelf == false) PauseMenu.gameObject.SetActive(true);
                if (MachineOfMadnessMap.gameObject.activeSelf == true) MachineOfMadnessMap.gameObject.SetActive(false);

            }
        }
        //controlling machine of madness transition
        else
        {
            if (TriviaMap.activeSelf == true) TriviaMap.gameObject.SetActive(false);
            if (LevelMap.activeSelf == true) LevelMap.gameObject.SetActive(false);
            if (MachineOfMadnessMap.gameObject.activeSelf == true) MachineOfMadnessMap.gameObject.SetActive(false);
        }

    }

    #region Cinematics
    public void OpenCinematic()
    {
        is_CachedCinematicMode = true;
        CurrentUI = UIMode.Cinematic;
    }

    public void CloseCinematic()
    {
        is_CachedCinematicMode = false;
        CurrentUI = UIMode.Default;

    }

    #endregion

    public void RefreshUI_Resolutions()
    {
        //now becomes 866 px the vertical minimum
        var refResolution = scaler_Main.referenceResolution;
        var UIScaling_Y = Mathf.Lerp(866f, 1080f, UI_Scaling);
        float absoluteMaxY = Mathf.Lerp(866f, 1080f, UI_Scaling * 1.5f);

        //if (Screen.height < UIScaling_Y)
        //    refResolution.y = Screen.height;
        //else
            refResolution.y = UIScaling_Y;

        if (refResolution.y < 866f)
            refResolution.y = 866f;

        scaler_Main.referenceResolution = refResolution;
        scaler_Pause.referenceResolution = refResolution;
        if (scaler_Trivia != null) scaler_Trivia.referenceResolution = refResolution; //remember title screen
    }

    public Vector3 GetMousePositionScaled()
    {
        Vector3 MousePos = Input.mousePosition;

        float width = Screen.width;
        float height = Screen.height;

        float ratio = scaler_Main.referenceResolution.y / height;
        //Debug.Log($"Mouse pos ratio: {ratio}");

        return MousePos * ratio;
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
            if (PauseMenu.activeSelf == true) PauseMenu.gameObject.SetActive(false);
            Time.timeScale = 1;
            event_GameResume?.Raise();
            HUD.gameObject.SetActive(true);
            ContextMenuUI.CloseContextMenu();
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (PauseMenu.activeSelf == false) PauseMenu.gameObject.SetActive(true);
            Time.timeScale = 0;
            event_GamePause?.Raise();

            HUD.gameObject.SetActive(false);


        }

        if (tempoPause == false)
        {
            //MainGameHUDScript.Instance.promptUIMain.SetActive(false);
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

    public void Demo_PlayLevel(ChamberLevel chamber)
    {
        Application.LoadLevel(chamber.scene.Index);
    }


    public void Demo_Restart()
    {
        Time.timeScale = 1;

        if (Hypatios.Game.currentGamemode.canSaveGame)
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
