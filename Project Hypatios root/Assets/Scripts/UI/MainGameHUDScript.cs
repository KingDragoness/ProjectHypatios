using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class MainGameHUDScript : MonoBehaviour
{

    public GameObject FadeOutSceneTransition;

    [Header("Player")]
    public Text healthPoint;
    public Text soulPoint;
    public GameObject NoAmmoAlert;
    public Slider healthSlider;
    public Slider justDamagedHealthSlider;
    public Slider dashSlider;
    public GameObject dashOk;
    public GameObject interactPrompt;
    public GameObject interactContainerPrompt;
    public Text interactText;

    [Header("Audio")]
    public AudioSource audio_DashReady;
    public AudioSource audio_CrosshairClick;
    public AudioSource audio_Error;
    public AudioSource audio_PurchaseReward;

    [Header("Trivias")]
    public UI_Modular_TypewriterText typewriter_Trivia;
    public GameObject triviaUI;

    [Header("Prompt UI")]
    public Text promptUIText;
    public Text promptUITitleText;
    public GameObject promptUIInputNameGroup;
    public GameObject promptUIMain;

    [Space]

    [Header("Weapon")]
    [ReadOnly] public List<Template_AmmoAddedIcon> AmmoAddedIcons;
    public Template_AmmoAddedIcon templatePrefab;
    public Transform parentNewAmmo;
    public Text currentAmmo;
    public Text maximumAmmo;
    public Recoil gunRecoil;
    public Image crosshairHit;
    public Image crosshairImage;
    public Image weaponUI;
    public Sprite defaultCrosshair;

    [Space]

    public ChargeStationUI chargeStationUI;
    public CraftingWorkstationUI craftingUI;

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

    private void OnEnable()
    {
        ReloadAmmoIcons();
    }

    private void Update()
    {
        soulPoint.text = $"{Hypatios.Game.SoulPoint}";
        //justDamagedHealthSlider.value = Mathf.MoveTowards(justDamagedHealthSlider.value, healthSlider.value, drainHealthSpeed * Time.deltaTime);
        HandleInteractable();
        HandleWeaponIcon();
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

        if (weapon == null) return;

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
    }

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
            var prefabAmmo = Instantiate(weaponClass.UI_TemplateAmmoAdded, parentNewAmmo);
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

    public void ShowPromptUI(string title, string text, bool isEnteringName = false)
    {
        promptUIMain.gameObject.SetActive(true);
        promptUIText.text = text;
        promptUITitleText.text = title;

        if (isEnteringName)
        { promptUIInputNameGroup.gameObject.SetActive(true); }
        else { promptUIInputNameGroup.gameObject.SetActive(false); }
    }


}
