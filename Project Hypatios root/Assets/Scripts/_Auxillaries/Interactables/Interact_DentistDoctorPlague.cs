using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interact_DentistDoctorPlague : MonoBehaviour
{
    [System.Serializable]
    public class PriceItem
    {
        public BaseStatusEffectObject statusEffect;
        public int price = 30;
    }

    public List<PriceItem> allPriceItems = new List<PriceItem>();
    public List<GameObject> buttonImplants = new List<GameObject>();
    public Interact_MultiDialoguesTrigger dialogueTriggerSuccess;
    public UnityEvent OnSuccessfulOperation;


    public void BuyItem(int index)
    {
        PriceItem priceList = allPriceItems[index];

        if (priceList.price > Hypatios.Game.SoulPoint)
        {
            DialogueSubtitleUI.instance.QueueDialogue($"Not enough souls!", "SYSTEM", 5f); 
            return;
        }

        Hypatios.Game.SoulPoint -= priceList.price;
        priceList.statusEffect.AddStatusEffectPlayer(9999f);

        DeadDialogue.PromptNotifyMessage_Mod($"Successful intenstine replacement.", 5f);
        OnSuccessfulOperation?.Invoke();

        foreach (var button in buttonImplants)
        {
            button.gameObject.SetActive(false);
        }

        Hypatios.UI.mainHUDScript.FadeIn();
        dialogueTriggerSuccess.TriggerMessage();
    }

}
