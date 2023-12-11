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
    public bool isConstantRefresh = false;
    [ShowIf("isConstantRefresh", true)] public float RefreshRate = 10; //10 times per second
    [ShowIf("showHealthBar", true, true)] public Slider slider_Health;
    [ShowIf("showHealthSpeed", true, true)] public Slider slider_HealthSpeed;
    [ShowIf("showHealthBar", true, true)] public Text label_Health;
    [ShowIf("showStatusEffects", true, true)] public RPG_CharPerkButton PerkButton;
    [ShowIf("showStatusEffects", true, true)] public RPG_CharPerkButton StatusMonoButton;
    [ShowIf("showStatusEffects", true, true)] public Transform parentPerks;

    private float _refreshTimer = 1f;
    private List<RPG_CharPerkButton> _allCharPerkButtons = new List<RPG_CharPerkButton>();
    private List<RPG_CharPerkButton> _allStatusMonoButtons = new List<RPG_CharPerkButton>();
    public static List<ModularUI_PlayerStats> AllPlayerStat = new List<ModularUI_PlayerStats>();

    private void OnEnable()
    {
        RefreshUI();
        _refreshTimer = 1f / RefreshRate;
        AllPlayerStat.Add(this);

    }

    public static void ForceRefreshAll()
    {
        foreach (var statUI in AllPlayerStat)
        {
            if (statUI == null) continue;
            statUI.RefreshUI();
        }

    }


    private void OnDisable()
    {
        AllPlayerStat.Remove(this);

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

    private void Update()
    {
        if (isConstantRefresh)
        {
            _refreshTimer -= Time.deltaTime;

            if (_refreshTimer <= 0f)
            {
                RefreshConstantStatusEffect();
                _refreshTimer = 1f / RefreshRate;
            }
        }
    }

    [ReadOnly] [ShowInInspector] private List<BaseModifierEffect> d_allCurrPerk = new List<BaseModifierEffect>();
    [ReadOnly] [ShowInInspector] private List<StatusEffectMono> d_allStatusMono = new List<StatusEffectMono>();

    private void RefreshConstantStatusEffect()
    {
        var allCurrentPerk = new List<BaseModifierEffect>(); 
        var AllStatusMono = new List<StatusEffectMono>();


        allCurrentPerk.AddRange(Hypatios.Player.AllStatusInEffect.FindAll(x => x.SourceID != "PermanentPerk"));
        AllStatusMono.AddRange(Hypatios.Player.AllStatusMonos);

        allCurrentPerk.RemoveAll(x => x.IsTiedToStatusMono == true);
        allCurrentPerk.RemoveAll(x => Hypatios.Assets.GetStatusEffect(x.statusCategoryType) == null);
        AllStatusMono.RemoveAll(x => Hypatios.Assets.GetStatusEffect(x.statusEffect.GetID()) == null);
        d_allCurrPerk = allCurrentPerk;
        d_allStatusMono = AllStatusMono;

        List<RPG_CharPerkButton> buttonToRemove = new List<RPG_CharPerkButton>();
        //check missing all status monos
        {
            foreach (var button in _allStatusMonoButtons)
            {
                if (button.baseStatusEffectGroup == null) continue;
                bool isStatusEffectExists = Hypatios.Player.AllStatusMonos.Find(x => x == button.attachedStatusEffectGO) != null ? true : false;

                if (isStatusEffectExists == false)
                {
                    buttonToRemove.Add(button);
                }
            }
        }

        {
            //foreach (var perkButton in _allCharPerkButtons)
            //{
            //    perkButton.gameObject.SetActive(false);
            //    //Destroy(perkButton.gameObject);
            //}

            //foreach (var statButton in buttonToRemove)
            //{
            //    statButton.gameObject.SetActive(false);
            //    //Destroy(statButton.gameObject);
            //}
        }

        _allCharPerkButtons.RemoveAll(x => x == null);
        _allStatusMonoButtons.RemoveAll(x => x == null);

        //Refresh perks
        for(int g = 0; g < allCurrentPerk.Count; g++)
        {
            var perk = allCurrentPerk[g];
            var statusEffect1 = Hypatios.Assets.GetStatusEffect(perk.statusCategoryType);
            RPG_CharPerkButton newButton = null;

            //Debug.Log($"{_allCharPerkButtons.Count}; {g}");

            //if 4 buttons but index is 4 (5 count)
            if (_allCharPerkButtons.Count > g)
            {
                newButton = _allCharPerkButtons[g];
            }
            else
            {
                newButton = Instantiate(PerkButton, parentPerks);
                _allCharPerkButtons.Add(newButton);
                newButton.gameObject.SetActive(true);
            }

            newButton.statusEffect = statusEffect1;
            newButton.attachedStatusEffectGO = perk;
            newButton.Refresh();
        }

        for(int v = allCurrentPerk.Count; v < _allCharPerkButtons.Count; v++)
        {
            if (_allCharPerkButtons.Count <= v) continue;
            var button = _allCharPerkButtons[v];

            button.gameObject.SetActive(false);
        }
        
        for (int g = 0; g < AllStatusMono.Count; g++)
        {
            var baseStatusEffect = AllStatusMono[g];
            var statusEffect1 = Hypatios.Assets.GetStatusEffect(baseStatusEffect.statusEffect.GetID());

            RPG_CharPerkButton newButton = null;

            if (_allStatusMonoButtons.Count > g)
            {
                newButton = _allStatusMonoButtons[g];
            }
            else
            {
                newButton = Instantiate(StatusMonoButton, parentPerks);
                _allStatusMonoButtons.Add(newButton);
                newButton.gameObject.SetActive(true);
            }

            newButton.baseStatusEffectGroup = statusEffect1;
            newButton.attachedStatusEffectGO = baseStatusEffect;
            newButton.Refresh();
        }

        for (int v = AllStatusMono.Count; v < _allStatusMonoButtons.Count; v++)
        {
            if (_allStatusMonoButtons.Count <= v) continue;
            var button = _allStatusMonoButtons[v];

            button.gameObject.SetActive(false);
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
