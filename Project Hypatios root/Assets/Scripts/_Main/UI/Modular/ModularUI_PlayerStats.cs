using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class ModularUI_PlayerStats : MonoBehaviour
{

    public bool showHealthBar = false;
    public bool showHealthSpeed = false;
    public bool showStatusEffects = false;
    [ShowIf("showHealthBar", true, true)] public Slider slider_Health;
    [ShowIf("showHealthSpeed", true, true)] public Slider slider_HealthSpeed;
    [ShowIf("showHealthBar", true, true)] public Text label_Health;
    [ShowIf("showStatusEffects", true, true)] public RPG_CharPerkButton PerkButton;
    [ShowIf("showStatusEffects", true, true)] public RPG_CharPerkButton StatusMonoButton;
    [ShowIf("showStatusEffects", true, true)] public Transform parentPerks;


    private List<RPG_CharPerkButton> _allCharPerkButtons = new List<RPG_CharPerkButton>();
    private List<RPG_CharPerkButton> _allStatusMonoButtons = new List<RPG_CharPerkButton>();

    private void OnEnable()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (showHealthBar)
        {
            string HP = $"{Mathf.RoundToInt(Hypatios.Player.Health.curHealth)}/{Mathf.RoundToInt(Hypatios.Player.Health.maxHealth.Value)}";
            slider_Health.value = Hypatios.Player.Health.curHealth;
            slider_Health.maxValue = Hypatios.Player.Health.maxHealth.Value;
            slider_HealthSpeed.value = Hypatios.Player.Health.targetHealth;
            slider_HealthSpeed.maxValue = Hypatios.Player.Health.maxHealth.Value;
            label_Health.text = HP;
        }

        if (showStatusEffects)
        {
            RefreshStatusEffects();
        }
    }



    public void RefreshStatusEffects()
    {
        List<RPG_CharPerkButton> buttonToRemove = new List<RPG_CharPerkButton>();
        //check missing all status monos
        {
            foreach(var button in _allStatusMonoButtons)
            {
                if (button.baseStatusEffectGroup == null) continue;
                bool isStatusEffectExists = Hypatios.Player.AllStatusMonos.Find(x => x == button.attachedStatusEffectGO) != null ? true : false; 

                if (isStatusEffectExists == false)
                {
                    buttonToRemove.Add(button);
                }
            }
        }
        foreach (var perkButton in _allCharPerkButtons)
        {
            Destroy(perkButton.gameObject);
        }

        foreach (var statButton in buttonToRemove)
        {
            Destroy(statButton.gameObject);
            _allStatusMonoButtons.Remove(statButton);
        }

        _allCharPerkButtons.Clear();
        _allStatusMonoButtons.RemoveAll(x => x == null);

        //Refresh perks
        {
            var allCurrentPerk = Hypatios.Player.AllStatusInEffect.FindAll(x => x.SourceID != "PermanentPerk"); //"TempPerk"
            allCurrentPerk.RemoveAll(x => x.IsTiedToStatusMono == true);

            foreach (var perk in allCurrentPerk)
            {
                var statusEffect1 = Hypatios.Assets.GetStatusEffect(perk.statusCategoryType);
                if (statusEffect1 == null) continue;
                var newButton = Instantiate(PerkButton, parentPerks);
                newButton.gameObject.SetActive(true);
                newButton.statusEffect = statusEffect1;
                newButton.attachedStatusEffectGO = perk;
                newButton.Refresh();
                _allCharPerkButtons.Add(newButton);
            }
        }
        {
            var AllStatusMono = Hypatios.Player.AllStatusMonos;

            foreach (var baseStatusEffect in AllStatusMono)
            {
                bool isButtonAlreadyExist = _allStatusMonoButtons.Find(l => l.attachedStatusEffectGO == baseStatusEffect) != null ? true : false;
                if (isButtonAlreadyExist)
                    continue;

                var statusEffect1 = Hypatios.Assets.GetStatusEffect(baseStatusEffect.statusEffect.GetID());
                if (statusEffect1 == null) 
                    continue;
                var newButton = Instantiate(StatusMonoButton, parentPerks);
                newButton.gameObject.SetActive(true);
                newButton.baseStatusEffectGroup = statusEffect1;
                newButton.attachedStatusEffectGO = baseStatusEffect;
                newButton.Refresh();
                _allStatusMonoButtons.Add(newButton);
            }
        }

    }

}
