using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class Interact_MiningExtractor : MonoBehaviour
{

    public UnityEvent OnActivateSuccess;

    public Interact_Container chest;
    [FoldoutGroup("Display")] public GameObject display_Inactive;
    [FoldoutGroup("Display")] public GameObject display_Active;
    [FoldoutGroup("Display")] public TextMesh text_DepositOre;
    [FoldoutGroup("Display")] public TextMesh text_ProductionRate;
    [FoldoutGroup("Display")] public TextMesh text_MiningProgress;
    [FoldoutGroup("Display")] public Interact_Touchable touchable;
    public int productionRate = 30; //30 ores per minute
    public int soulPrice = 20;
    public LootTable lootTable;
    [ReadOnly] public bool activated = false;

    private float _drillTimer = 2f;
    private float _UIUpdateTimer = 0.1f;

    private void Start()
    {
        _drillTimer = 60f / productionRate;
    }

    private void Update()
    {
        if (Time.timeScale <= 0) return;
        _UIUpdateTimer -= Time.deltaTime;

        if (_UIUpdateTimer < 0)
        {
            UpdateUI();
        }

        if (activated == true)
        {
            _drillTimer -= Time.deltaTime;
     

            if (_drillTimer < 0)
            {
                AddItemToChest();
                _drillTimer = 60f / productionRate;
            }
        }
        else
        {

        }
    }

    public void BuyExtractor()
    {
        if (activated)
        {
            Hypatios.Dialogue.QueueDialogue($"Extractor already active.", "SYSTEM", 3f, shouldOverride: true);
            return;
        }

        if (Hypatios.Game.SoulPoint < soulPrice)
        {
            Hypatios.Dialogue.QueueDialogue($"Not enough souls! Required: {soulPrice} souls.", "SYSTEM", 3f, shouldOverride: true);
            return;
        }

        DeadDialogue.PromptNotifyMessage_Mod($"Successful purchase of Mining Extractor: {soulPrice} souls.", 6f);
        Hypatios.Game.SoulPoint -= soulPrice;
        OnActivateSuccess?.Invoke();
        activated = true;
        UpdateUI();
    }


    private void UpdateUI()
    {
        if (activated == true)
        {
            float mineTime = 60f / productionRate;
            float percent = 100f - Mathf.Floor(_drillTimer / mineTime * 100);
            text_MiningProgress.text = $"MINING...{Mathf.RoundToInt(percent)}%";

            if (display_Inactive.activeSelf == true) display_Inactive.gameObject.SetActive(false);
            if (display_Active.activeSelf == false) display_Active.gameObject.SetActive(true);
        }
        else
        {
            text_ProductionRate.text = $"{productionRate} drills/minute";
            touchable.interactDescription = $"{soulPrice} souls";

            if (display_Inactive.activeSelf == false) display_Inactive.gameObject.SetActive(true);
            if (display_Active.activeSelf == true) display_Active.gameObject.SetActive(false);
        }

        string s1 = "";

        foreach(var entry in lootTable.entries)
        {
            float percent = lootTable.GetPercentage(entry);
            s1 += $"{entry.item.GetDisplayText()} {Mathf.RoundToInt(percent*100)}%\n";
        }

        text_DepositOre.text = s1;
    }

    private void AddItemToChest()
    {
        var entry = lootTable.GetEntry();
        chest.inventory.AddItem(entry.item, 1);
    }

}
