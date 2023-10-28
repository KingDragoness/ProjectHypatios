using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class MainGameHUDScript : MonoBehaviour
{

    public GameObject FadeOutSceneTransition;
    public GameObject FadeInSceneTransition;
    public RectTransform mainHUDParent;

    [FoldoutGroup("Title Card")] public UI_Modular_TypewriterText titlecard_Title;
    [FoldoutGroup("Title Card")] public UI_Modular_TypewriterText titlecard_Description;
    [FoldoutGroup("Title Card")] public GameObject TitleCard;
    [FoldoutGroup("Demo")] public Text demo_Label_TimeLimit;
    [FoldoutGroup("Demo")] public GameObject demo_HUD;
    [FoldoutGroup("Demo")] public GameObject demo_DemoFinished;

    [FoldoutGroup("Player")] public Text healthPoint;
    [FoldoutGroup("Player")] public Text soulPoint;
    [FoldoutGroup("Player")] public GameObject NoAmmoAlert;
    [FoldoutGroup("Player")] public Slider healthSlider;
    [FoldoutGroup("Player")] public Slider justDamagedHealthSlider;
    [FoldoutGroup("Player")] public Slider dashSlider;
    [FoldoutGroup("Player")] public GenericBossUI bossUI;
    [FoldoutGroup("Player")] public GameObject dashOk;
    [FoldoutGroup("Player")] public GameObject interactPrompt;
    [FoldoutGroup("Player")] public GameObject interactContainerPrompt;
    [FoldoutGroup("Player")] public Text interactText;

    [FoldoutGroup("Audios")] public AudioSource audio_DashReady;
    [FoldoutGroup("Audios")] public AudioSource audio_CrosshairClick;
    [FoldoutGroup("Audios")] public AudioSource audio_Error;
    [FoldoutGroup("Audios")] public AudioSource audio_PurchaseReward;
    [FoldoutGroup("Audios")] public AudioSource audio_interactMonitor;
    [FoldoutGroup("Audios")] public AudioSource audio_SuccessfulPayment;


    [FoldoutGroup("Trivias")] public UI_Modular_TypewriterText typewriter_Trivia;
    [FoldoutGroup("Trivias")] public GameObject triviaUI;

    [FoldoutGroup("Prompt")] public Text promptUIText;
    [FoldoutGroup("Prompt")] public Text promptUITitleText;
    [FoldoutGroup("Prompt")] public GameObject promptUIInputNameGroup;
    [FoldoutGroup("Prompt")] public PromptHelperUI promptUIMain;

    [Space]

    [Header("Weapon")]
    [FoldoutGroup("Weapons")] [ReadOnly] public List<Template_AmmoAddedIcon> AmmoAddedIcons;
    [FoldoutGroup("Weapons")] public Template_AmmoAddedIcon templatePrefab;
    [FoldoutGroup("Weapons")] public Transform parentNewAmmo;
    [FoldoutGroup("Weapons")] public Text currentAmmo;
    [FoldoutGroup("Weapons")] public Text maximumAmmo;
    [FoldoutGroup("Weapons")] public Recoil gunRecoil;
    [FoldoutGroup("Weapons")] public Image crosshairHit;
    [FoldoutGroup("Weapons")] public Image crosshairImage;
    [FoldoutGroup("Weapons")] public Image reloadImageCircle;
    [FoldoutGroup("Weapons")] public UI_Modular_CanvasOpacityController opacityReload;
    [FoldoutGroup("Weapons")] public Image weaponUI;
    [FoldoutGroup("Weapons")] public Sprite defaultCrosshair;

    [Space]

    public ChargeStationUI chargeStationUI;
    public CraftingWorkstationUI craftingUI;
    public kThanidLabUI kthanidUI;
    public LootedItemNotifyUI lootItemUI;
    public PlayerRPGUI rpgUI;

    public static MainGameHUDScript Instance
    {
        get
        {
            return Hypatios.UI.mainHUDScript;
        }
    }

    private void Awake()
    {

    }

    private void Start()
    {
        bool showTitleCard = false;
        var chamberSO = Hypatios.Chamber.chamberObject;

        if (chamberSO != null)
        {
            if (chamberSO.showTitleCard)
            {
                titlecard_Title.text_DialogueContent.text = "";
                titlecard_Description.text_DialogueContent.text = "";
                StartCoroutine(ActivateTypewriter());
                showTitleCard = true;
            }
        }

        if (showTitleCard)
        {
            TitleCard.gameObject.SetActive(true);
        }
        else
        {
            TitleCard.gameObject.SetActive(false);
        }
    }

    IEnumerator ActivateTypewriter()
    {
        yield return new WaitForSeconds(1f);
        var chamberSO = Hypatios.Chamber.chamberObject;

        titlecard_Title.TypeThisDialogue($"\"{chamberSO.TitleCard_Title.ToUpper()}\"");
        titlecard_Description.TypeThisDialogue($"{chamberSO.TitleCard_Subtitle}");
    }

    private void OnEnable()
    {
        ReloadAmmoIcons();
    }

    #region Update

    private void Update()
    {
        soulPoint.text = $"{Hypatios.Game.SoulPoint}";
        //justDamagedHealthSlider.value = Mathf.MoveTowards(justDamagedHealthSlider.value, healthSlider.value, drainHealthSpeed * Time.deltaTime);
        HandleInteractable();
        HandleWeaponIcon();
        DemoUpdate();
    }

    private void HandleInteractable()
    {

        bool hidePrompt = true;
        bool containerExists = false;

        if (InteractableCamera.instance != null)
        {
            if (InteractableCamera.instance.currentInteractable != null)
            {
                hidePrompt = false;

                if (InteractableCamera.instance.currentInteractable is Interact_Container)
                    containerExists = true;
            }
        }

        if (containerExists)
        {
            if (!interactContainerPrompt.activeSelf) interactContainerPrompt.gameObject.SetActive(true);

        }
        else
        {
            if (interactContainerPrompt.activeSelf) interactContainerPrompt.gameObject.SetActive(false);
        }

        if (hidePrompt)
        {
            interactPrompt.gameObject.SetActive(false);
        }
        else
        {
            if (!interactPrompt.activeSelf)
            {
                interactPrompt.gameObject.SetActive(true);

            }

            try
            {
                var descript = InteractableCamera.instance.currentInteractable.GetDescription();
                interactText.text = $"[E] - {descript}";

            }
            catch
            {
                interactText.text = "[E] - Interact";
            }
        }

    }

    private void HandleWeaponIcon()
    {
        GunScript weapon = Hypatios.Player.Weapon.currentGunHeld;

        if (weapon == null)
        {
            opacityReload.isVisible = false;
            return;
        }

        if (weapon.lowAmmoAlert >= weapon.curAmmo)
        {
            if (!NoAmmoAlert.activeSelf)
                NoAmmoAlert.gameObject.SetActive(true);

        }
        else
        {
            if (NoAmmoAlert.activeSelf)
                NoAmmoAlert.gameObject.SetActive(false);
        }

        if (weapon.isReloading)
        {
            float _reloadTime = weapon.ReloadTime;
            float _curTimerReload = weapon.curReloadTime;
            reloadImageCircle.fillAmount = _curTimerReload / _reloadTime;
            opacityReload.isVisible = true;
        }
        else
        {
            opacityReload.isVisible = false;
        }
    }

    private void DemoUpdate()
    {
        if (Hypatios.IsDemoMode == false)
        {
            if (demo_HUD.gameObject.activeSelf == true) demo_HUD.gameObject.SetActive(false);
            if (demo_DemoFinished.gameObject.activeSelf == true) demo_DemoFinished.gameObject.SetActive(false);
            return;
        }

        if (demo_HUD.gameObject.activeSelf == false) demo_HUD.gameObject.SetActive(true);

        var rt = (Hypatios.DemoTrialTimeLimit - Hypatios.Game.Total_UNIX_Timespan);
        if (rt <= 0) rt = 0;
        var dateTime = ClockTimerDisplay.UnixTimeStampToDateTime(rt, true);
        int extraMinute = (dateTime.Hour * 60) + dateTime.Minute;
        demo_Label_TimeLimit.text = $"{extraMinute} minutes";
        //label_Countdown.text = $"{ dateTime.Minute.ToString("00")}:{ dateTime.Second.ToString("00")}";

        if (Hypatios.IsDemoFinished() == true)
        {
            if (demo_DemoFinished.gameObject.activeSelf == false) demo_DemoFinished.gameObject.SetActive(true);

        }
    }

    #endregion

    public void PlayDash()
    {
        audio_DashReady.Play();
    }

    public void SwapCrosshair(Sprite crosshair)
    {
        if (crosshair != null)
        {
            crosshairImage.sprite = crosshair;
        }
        else
        {
            crosshairImage.sprite = defaultCrosshair;
        }
    }

    public void ReloadAmmoIcons()
    {
        foreach (var iconAmmo in AmmoAddedIcons)
        {
            if (iconAmmo == null) continue;
            Destroy(iconAmmo.gameObject);
        }

        AmmoAddedIcons.Clear();

        foreach (var weapon in Hypatios.Player.Weapon.CurrentlyHeldWeapons)
        {
            var weaponClass = Hypatios.Assets.GetWeapon(weapon.weaponName);
            if (weaponClass.UI_TemplateAmmoAdded == null) continue;
            var prefabAmmo = Instantiate(weaponClass.UI_TemplateAmmoAdded, parentNewAmmo); //dasar engine kimak
            if (prefabAmmo == null) continue;
            AmmoAddedIcons.Add(prefabAmmo);
        }
    }
    public void ShowAmmo(string weaponName, int count)
    {
        Template_AmmoAddedIcon targetIcon = AmmoAddedIcons.Find(x => x.weaponID == weaponName);
        targetIcon.SetAmmoText($"+{count}");
        targetIcon.TriggerAnim();

    }

    public void ShowPromptUI(string title, string text, bool isEnteringName = false, float _time = 7.5f)
    {
        promptUIMain.gameObject.SetActive(false);
        promptUIMain.gameObject.SetActive(true);
        promptUIText.text = text;
        promptUITitleText.text = title;

        if (isEnteringName)
        { promptUIInputNameGroup.gameObject.SetActive(true); }
        else { promptUIInputNameGroup.gameObject.SetActive(false); }
    }

    public void FadeIn()
    {
        FadeInSceneTransition.gameObject.SetActive(false);
        FadeInSceneTransition.gameObject.SetActive(true);
    }

    public GameObject AttachModularUI(GameObject modularUI)
    {
        var newWindow = Instantiate(modularUI, mainHUDParent.transform);
        newWindow.transform.localScale = Vector3.one;


        return newWindow;
    }


}
