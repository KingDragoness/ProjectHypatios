using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGameHUDScript : MonoBehaviour
{

    public GameObject FadeOutSceneTransition;

    [Header("Player")]
    public Text healthPoint;
    public Text soulPoint;
    public Slider healthSlider;
    public Slider justDamagedHealthSlider;
    public Slider dashSlider;
    public GameObject dashOk;
    public GameObject interactPrompt;
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
    public List<Template_AmmoAddedIcon> AmmoAddedIcons;
    public Text currentAmmo;
    public Text maximumAmmo;
    public Recoil gunRecoil;
    public Image crosshairHit;
    public Image crosshairImage;
    public Image weaponUI;
    public Sprite defaultCrosshair;

    [Space]

    public ChargeStationUI chargeStationUI;

    public static MainGameHUDScript Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        soulPoint.text = $"{Hypatios.Game.SoulPoint}";
        //justDamagedHealthSlider.value = Mathf.MoveTowards(justDamagedHealthSlider.value, healthSlider.value, drainHealthSpeed * Time.deltaTime);

        bool hidePrompt = true;

        if (InteractableCamera.instance != null)
        {
            if (InteractableCamera.instance.currentInteractable != null)
            {
                hidePrompt = false;
            }
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

    public void ShowAmmo(string weaponName, int count)
    {
        WeaponItem.Category category = WeaponItem.Category.Pistol;

        switch(weaponName)
        {
            case "Pistol":
                category = WeaponItem.Category.Pistol;
                break;

            case "Shotgun":
                category = WeaponItem.Category.Shotgun;
                break;

            case "SMG":
                category = WeaponItem.Category.SMG;
                break;

            case "Rifle":
                category = WeaponItem.Category.Rifle;
                break;

            default:

                break;
        }

        Template_AmmoAddedIcon targetIcon = AmmoAddedIcons.Find(x => x.category == category);
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
