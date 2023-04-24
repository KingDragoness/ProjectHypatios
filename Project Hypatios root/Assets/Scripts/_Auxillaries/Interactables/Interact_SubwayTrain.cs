using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DevLocker.Utils;

public class Interact_SubwayTrain : MonoBehaviour
{

    [System.Serializable]
    public class PriceShortcut
    {
        public SceneReference sceneTarget;
        public int souls = 10;
    }

    public Interact_SubwayTrain_DestinationSelect currentDestination;
    public List<Interact_SubwayTrain_DestinationSelect> allDestinationsButton = new List<Interact_SubwayTrain_DestinationSelect>();
    public List<PriceShortcut> priceList = new List<PriceShortcut>();
    public UnityEvent OnSceneTriggered;

    private bool isLoading = false;

    private void OnEnable()
    {

        var perkClass = HypatiosSave.PerkDataSave.GetPerkDataSave();

        foreach (var button in allDestinationsButton)
        {
            PriceShortcut priceShortcut = GetPriceList(button.sceneTarget);

            if (priceShortcut != null)
            {
                var level = perkClass.Perk_LV_ShortcutDiscount;
                float discount = 1 + PlayerPerk.GetBonusShortcutDiscount(level);
                int price = Mathf.RoundToInt(discount * priceShortcut.souls);

                button.touchScript.interactDescription = $"{price} souls";
            }
        }
    }

    private PriceShortcut GetPriceList(SceneReference sceneReference)
    {
        return priceList.Find(x => x.sceneTarget.SceneName == sceneReference.SceneName);
    }

    private void Update()
    {
        if (Time.timeScale == 0) return;

        foreach(var button in allDestinationsButton)
        {
            button.Unactive();
        }

        if (currentDestination != null)
            currentDestination.Activate();
    }

    private bool NotASingleDestinationActive()
    {
        bool b = true;

        foreach(var button in allDestinationsButton)
        {
            if (button.gameObject.activeSelf)
                return false;
        }

        return b;
    }


    [ContextMenu("Trigger Scene")]
    public void SceneTrigger()
    {
        if (NotASingleDestinationActive() == true)
        {
            DialogueSubtitleUI.instance.QueueDialogue("No destination available right now, come back when you have progressed the game further.", "SYSTEM", 5.5f);
            return;
        }

        if (currentDestination == null)
        {
            DialogueSubtitleUI.instance.QueueDialogue("Please select a destination first.", "SYSTEM", 4f);
            return;
        }

        PriceShortcut priceShortcut = GetPriceList(currentDestination.sceneTarget);

        if (currentDestination.sceneTarget == null | priceShortcut == null)
        {
            Debug.LogError("Missing destination target/price list null!");
            return;
        }
        
        if (priceShortcut.souls > Hypatios.Game.SoulPoint)
        {
            DialogueSubtitleUI.instance.QueueDialogue($"Not enough souls for your destination! Price: {priceShortcut.souls}", "SYSTEM", 4f);
            return;
        }


        if (isLoading) return;

        StartCoroutine(LoadLevel());
    }

    private IEnumerator LoadLevel()
    {
        int target = 0;
        var perkClass = HypatiosSave.PerkDataSave.GetPerkDataSave();

        if (currentDestination.sceneTarget == null) yield break;
        var level = perkClass.Perk_LV_ShortcutDiscount;
        float discount = 1 + PlayerPerk.GetBonusShortcutDiscount(level);
        PriceShortcut priceShortcut = GetPriceList(currentDestination.sceneTarget);
        int netPrice = Mathf.RoundToInt(priceShortcut.souls * discount);

        Hypatios.Game.SoulPoint -= netPrice;
        OnSceneTriggered?.Invoke();
        isLoading = true;

        target = currentDestination.sceneTarget.Index;
        Debug.Log($"{currentDestination.sceneTarget.SceneName} = ({currentDestination.sceneTarget.Index})");

        DialogueSubtitleUI.instance.QueueDialogue($"Destination: {currentDestination.sceneTarget.SceneName}. {netPrice} souls has been deducted.", "SYSTEM", 10f);

        yield return new WaitForSeconds(2f);
        MainGameHUDScript.Instance.FadeOutSceneTransition.gameObject.SetActive(true);
        Hypatios.Game.SaveGame(targetLevel: target);
        yield return new WaitForSeconds(2f);
        Hypatios.Game.BufferSaveData();



        Application.LoadLevel(target);

    }

}
