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
    [ShowIf("isPermanent")] public StatusEffectCategory statusType;
    public Interact_Touchable touch_TakePerk;
    public Interact_Touchable touch_Inspect;
    public Interact_TutorialBubble dialogInspect;
    public GameObject activeMode;
    public GameObject unactiveMode;
    public Image icon;

    public void Inspect()
    {
        dialogInspect.TriggerMessage();
    }

    public void BuyPerk()
    {
        offerPerkScript.BuyPerk(this);
    }

    public void RewardPerk()
    {
        offerPerkScript.RewardPerk(this);
    }

    public void Deactivate()
    {
        activeMode.gameObject.SetActive(false);
        unactiveMode.gameObject.SetActive(true);
    }
}
