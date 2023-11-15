using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class Interact_PerkOffer_Cauldron : MonoBehaviour
{

    public Interact_PerkOffering offerPerkScript;
    public bool isPermanent = false;
    [HideIf("isPermanent")] public PerkCustomEffect perkCustomEffect;
    [ShowIf("isPermanent")] public ModifierEffectCategory statusType;
    public Interact_Touchable touch_TakePerk;
    public Interact_Touchable touch_Inspect;
    public Interact_TutorialBubble dialogInspect;
    public GameObject activeMode;
    public GameObject unactiveMode;
    public Image icon;
    public Text label_Title;
    public Text label_Description;

    private bool isDeactive = false;

    private void OnEnable()
    {
        if (isDeactive)
        {
            if (activeMode.gameObject.activeSelf) activeMode.gameObject.SetActive(false);
            if (unactiveMode.gameObject.activeSelf == false) unactiveMode.gameObject.SetActive(true);
        }
        else
        {
            if (activeMode.gameObject.activeSelf == false) activeMode.gameObject.SetActive(true);
            if (unactiveMode.gameObject.activeSelf) unactiveMode.gameObject.SetActive(false);
        }
    }

    public void Inspect()
    {
        //dialogInspect.TriggerMessage();
        DeadDialogue.PromptNotifyMessage_Mod(dialogInspect.Dialogue_Content, 5f);
    }

    public void BuyPerk()
    {
        offerPerkScript.BuyPerk(this);
    }

    public void RewardPerk()
    {
        offerPerkScript.RewardPerk(this);
    }

    public void ResetState()
    {
        isDeactive = false;
        activeMode.gameObject.SetActive(true);
        unactiveMode.gameObject.SetActive(false);
    }

    public void Deactivate()
    {
        isDeactive = true;
        activeMode.gameObject.SetActive(false);
        unactiveMode.gameObject.SetActive(true);
    }
}
