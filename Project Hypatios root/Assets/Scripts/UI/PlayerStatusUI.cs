using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Sirenix.OdinInspector;


public class PlayerStatusUI : MonoBehaviour
{

    [FoldoutGroup("Perk Tooltip")] public ToolTip smallTooltipUI;
    [FoldoutGroup("Perk Tooltip")] public ToolTip bigTooltipUI;
    [FoldoutGroup("Perk Tooltip")] public Text smallTooltip_LeftHandedLabel;
    [FoldoutGroup("Perk Tooltip")] public Text smallTooltip_RightHandedLabel;
    [FoldoutGroup("Perk Tooltip")] public Text bigTooltip_LeftHandedLabel;
    [FoldoutGroup("Perk Tooltip")] public Text bigTooltip_RightHandedLabel;
    [FoldoutGroup("Perk Tooltip")] public Vector3 offsetTooltip;



    public RPG_CharPerkButton PerkButton;
    public RPG_CharPerkButton StatusMonoButton;
    public RPG_CharPerkButton currentPerkButton;
    public Transform parentPerks;
    public Text hp_Label;
    public Slider hp_Slider;

    private List<RPG_CharPerkButton> _allCharPerkButtons = new List<RPG_CharPerkButton>();
    private List<RPG_CharPerkButton> _allStatusMonoButtons = new List<RPG_CharPerkButton>();
    private Canvas pauseCanvas;



    private void OnEnable()
    {
        RefreshUI();
        pauseCanvas = GetComponentInParent<Canvas>();
    }

    private void RefreshUI()
    {
        foreach (var perkButton in _allCharPerkButtons)
        {
            Destroy(perkButton.gameObject);
        }
        foreach (var statButton in _allStatusMonoButtons)
        {
            Destroy(statButton.gameObject);
        }
        
        _allCharPerkButtons.Clear();
        _allStatusMonoButtons.Clear();

        string HP = $"{Mathf.RoundToInt(Hypatios.Player.Health.curHealth)}/{Mathf.RoundToInt(Hypatios.Player.Health.maxHealth.Value)}";
        hp_Label.text = HP;
        hp_Slider.value = Hypatios.Player.Health.curHealth;
        hp_Slider.maxValue = Hypatios.Player.Health.maxHealth.Value;


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
                var statusEffect1 = Hypatios.Assets.GetStatusEffect(baseStatusEffect.statusEffect.GetID());
                if (statusEffect1 == null) continue;
                var newButton = Instantiate(StatusMonoButton, parentPerks);
                newButton.gameObject.SetActive(true);
                newButton.baseStatusEffectGroup = statusEffect1;
                newButton.attachedStatusEffectGO = baseStatusEffect;
                newButton.Refresh();
                _allCharPerkButtons.Add(newButton);
            }
        }

        string s = "";

        var charStatButtons = GetComponentsInChildren<CharStatButton>();

        foreach (var button in charStatButtons)
        {
            button.ForceRefresh();
        }
    }

    #region Highlight/Dehighlight
    public void HighlightPerk(RPG_CharPerkButton _currentPerk)
    {
        currentPerkButton = _currentPerk;
        string timerString = $"{(Mathf.RoundToInt(currentPerkButton.attachedStatusEffectGO.EffectTimer * 10f) / 10f)}";
        if (currentPerkButton.attachedStatusEffectGO.EffectTimer >= 9999f) timerString = $"";
        else timerString = $"({timerString}s)";

        if (_currentPerk.type == RPG_CharPerkButton.Type.TemporaryModifier)
        {
            var value = currentPerkButton.attachedStatusEffectGO.Value;
            smallTooltip_LeftHandedLabel.text = $"{currentPerkButton.statusEffect.TitlePerk} {timerString}";

            if (value == 0 | value == -1)
            {
                smallTooltip_RightHandedLabel.text = "";
            }
            else
            {
                smallTooltip_RightHandedLabel.text = RPG_CharPerkButton.GetDescription(_currentPerk.statusEffect.category, value);
            }

            Hypatios.UI.ShowTooltipSmall(_currentPerk.GetComponent<RectTransform>());
        }
        else
        {
            var statEffectGroup = _currentPerk.baseStatusEffectGroup;
            string str1 = $"{statEffectGroup.GetDisplayText()} <size=13>{timerString}</size>\n<size=13>{statEffectGroup.Description}</size>";
            string str2 = "";

            foreach (var modifier in statEffectGroup.allStatusEffects)
            {
                var baseModifier = Hypatios.Assets.GetStatusEffect(modifier.statusCategoryType);
                str2 += $"[{baseModifier.TitlePerk}] [{RPG_CharPerkButton.GetDescription(modifier.statusCategoryType, modifier.Value)}]\n";
            }
            str2 = ""; //scrapped modifier text
            bigTooltip_LeftHandedLabel.text = $"{str1}\n<size=13>{str2}</size>";
            bigTooltip_RightHandedLabel.text = "";
            Hypatios.UI.ShowTooltipBig(_currentPerk.GetComponent<RectTransform>());

        }
    }

    public void DehighlightPerk()
    {
        currentPerkButton = null;
    }

    #endregion
}
