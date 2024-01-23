using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//Now shared with sacrificial chamber
public class Interact_DentistDoctorPlague : MonoBehaviour
{
    [System.Serializable]
    public class PriceItem
    {
        public BaseStatusEffectObject statusEffect;
        public UnityEvent OnOrganModify;
        public int price = 30;
    }

    public List<PriceItem> allPriceItems = new List<PriceItem>();
    public List<GameObject> buttonImplants = new List<GameObject>();
    public Interact_MultiDialoguesTrigger dialogueTriggerSuccess;
    public string prompt_stringMessage = "Successful intestine replacement.";
    public string prompt_stringMessageFail = "You already have an intestine.";
    public float prompt_Timer = 5f;
    public bool shouldDeactiveButtons = true;
    public UnityEvent OnSuccessfulOperation;


    public void BuyItem(int index)
    {
        PriceItem priceList = allPriceItems[index];

        if (Hypatios.Player.IsStatusEffectGroup(priceList.statusEffect) == true)
        {
            DeadDialogue.PromptNotifyMessage_Mod($"{prompt_stringMessageFail}", 5f);
            return;
        }

        if (priceList.price > Hypatios.Game.SoulPoint)
        {
            DeadDialogue.PromptNotifyMessage_Mod($"Not enough souls!", 5f); 
            return;
        }

        Hypatios.Game.SoulPoint -= priceList.price;
        priceList.statusEffect.AddStatusEffectPlayer(9999f);

        DeadDialogue.PromptNotifyMessage_Mod(prompt_stringMessage, prompt_Timer);
        OnSuccessfulOperation?.Invoke();

        if (shouldDeactiveButtons)
        {
            foreach (var button in buttonImplants)
            {
                button.gameObject.SetActive(false);
            }
        }

        Hypatios.UI.mainHUDScript.FadeIn();
        priceList.OnOrganModify?.Invoke();

        if (dialogueTriggerSuccess != null) dialogueTriggerSuccess.TriggerMessage();
    }

}
