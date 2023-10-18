using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class PerkSelectionUI : MonoBehaviour
{

    [FoldoutGroup("Event Flag")] public GlobalFlagSO flag_KillerPill;

    public DeathScreenScript deathScreenScript;
    public DieUI_PerkButton prefabPerkButton;
    public DieUI_PerkButton selectedPerkButton;
    public List<ModifierEffectCategory> allStatusUpgradables = new List<ModifierEffectCategory>();
    public RectTransform parentPerkList;

    private List<DieUI_PerkButton> allPerkButtons = new List<DieUI_PerkButton>();

    private void Start()
    {
        prefabPerkButton.gameObject.SetActive(false);
        RefreshPerkUI();
    }

    public void SelectPerkButton(DieUI_PerkButton currentButton)
    {
        selectedPerkButton = currentButton;
        deathScreenScript.currentReaperStage = DeathScreenScript.ReaperStage.Main;
    }

    [FoldoutGroup("Debug")]
    [Button("Test random Perk")]
    public void PerkTest(int loopRun = 100)
    {
        List<string> basePerkNames = new List<string>();

        for (int x = 0; x < loopRun; x++)
        {
            var basePerk1 = PlayerPerk.RandomPickBasePerk();
            basePerkNames.Add(basePerk1.name);
        }

        foreach(var basePerk in Hypatios.Assets.AllBasePerks)
        {
            var NewList = new List<string>();
            foreach (var entry in basePerkNames) NewList.Add(entry);
            NewList.RemoveAll(x => x != basePerk.name);
            Debug.Log($"{basePerk.name} = {NewList.Count}");
        }
    }

    [FoldoutGroup("Debug")]
    [Button("Refresh Perk")]

    public void RefreshPerkUI()
    {
        foreach (var button in allPerkButtons) Destroy(button.gameObject);
        allPerkButtons.Clear();

        int perkAmount = Random.Range(4, 7);

        if (FPSMainScript.savedata.Game_TotalRuns < 20)
        {
            perkAmount = Random.Range(3, 6);
        }

        for (int x = 0; x < perkAmount; x++)
        {
            float chance1 = Random.Range(0f, 1f);
            var newButton = Instantiate(prefabPerkButton, parentPerkList.transform);
            newButton.gameObject.SetActive(true);
            bool generateTempPerk = false;

            if (CheckFlagExist(flag_KillerPill)) chance1 = 0.9f;
            if (chance1 > 0.66f) generateTempPerk = true;

            if (generateTempPerk)
            {
                var statusTarget = PlayerPerk.RandomPickBaseTempPerk().category;

                newButton.customEffect.statusCategoryType = statusTarget;
                newButton.status = newButton.customEffect.statusCategoryType;
                newButton.customEffect.Generate("DeathScreen");
                newButton.isTemporaryPerk = true;
            }
            else
            {
                newButton.status = PlayerPerk.RandomPickBasePerk().category;
                int runLoop = 0;
                bool valid = false;

                if (IsDuplicatePerkSelect(newButton.status) == false) valid = true;

                while (valid == false)
                {
                    newButton.status = PlayerPerk.RandomPickBasePerk().category;
                    if (IsDuplicatePerkSelect(newButton.status) == false) valid = true;
                    if (runLoop > 100) break;
                    runLoop++;
                }
            }

            newButton.RefreshPerk();
            allPerkButtons.Add(newButton);
        }
    }

    public bool CheckFlagExist(GlobalFlagSO flag)
    {
        var saveData = Hypatios.GetHypatiosSave();
        var flagDat = saveData.Game_GlobalFlags.Find(x => x.ID == flag.GetID());

        if (flagDat != null)
            return true;

        return false;
    }

    public bool IsDuplicatePerkSelect(ModifierEffectCategory _category)
    {
        if (allPerkButtons.Find(x => x.status == _category) != null)
        {
            return true;
        }

        return false;
    }

    public ModifierEffectCategory RandomSelectStatus()
    {
        return allStatusUpgradables[Random.Range(0, allStatusUpgradables.Count)];
    }

    public ModifierEffectCategory RandomSelectStatusNoTemp()
    {
        var newList = new List<ModifierEffectCategory>(); foreach (var entry in allStatusUpgradables) newList.Add(entry);
        newList.RemoveAll(x => x == ModifierEffectCategory.SoulBonus);
        return newList[Random.Range(0, newList.Count)];
    }


}
