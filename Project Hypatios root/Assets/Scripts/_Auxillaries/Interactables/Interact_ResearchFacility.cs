using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Interact_ResearchFacility : MonoBehaviour
{
    
    [System.Serializable]
    public class Upgradeable
    {
        public List<BaseStatusEffectObject> upgradePerks = new List<BaseStatusEffectObject>();
        public float baseTime = 120f;
        public int baseSoul = 16;
        public float perLevel_Time = 24f;
        public int perLevel_Soul = 3;

        private int _currentLevel = 0;

        public int CurrentLevel { get => _currentLevel; }

        public void Refresh()
        {
            for(int x = 0; x < upgradePerks.Count; x++)
            {
                if (Hypatios.Player.IsStatusEffectGroup(upgradePerks[x]) == true)
                {
                    _currentLevel = x + 1;
                    return;
                }
            }

            _currentLevel = 0;
        }

        public bool HasLevelMaxed()
        {
            if (_currentLevel >= upgradePerks.Count)
            {
                return true;
            }

            return false;
        }

        public BaseStatusEffectObject GetResearchTarget()
        {
            Refresh();

            if (_currentLevel == 0)
                return upgradePerks[0];

            return upgradePerks[_currentLevel];
        }
    }

    public enum Mode
    {
        Idle,
        Researching
    }

    public List<Upgradeable> Upgradeables = new List<Upgradeable>();
    public Mode currentMode;
    public BaseStatusEffectObject currentlyUpgrade;
    public TextMesh label_Progress;
    public TextMesh label_Upgradename;
    public AudioSource audio_researchCompleted;
    public float DEBUG_researchMultiplierSpeed = 1f;

    public GameObject monitor_SelectUpgrade;
    public GameObject monitor_Research;
    [FoldoutGroup("Research Select")] public Interact_Touchable touch_weapon;
    [FoldoutGroup("Research Select")] public Interact_Touchable touch_armor;

    private float _cooldownUpdateUI = 0.5f;
    private float _researchTime = 0f;
    private float _originalResearchTime = 0f;
    private int _currentIndex = 0;
    public static List<Interact_ResearchFacility> allResearchFacilities = new List<Interact_ResearchFacility>();

    private void Start()
    {
        monitor_SelectUpgrade.gameObject.SetActive(true);
        monitor_Research.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        allResearchFacilities.Add(this);
    }

    private void OnDisable()
    {
        allResearchFacilities.Remove(this);
    }

    private void Update()
    {

        if (currentMode == Mode.Researching)
        {
            HandleResearch();
        }

        _cooldownUpdateUI -= Time.deltaTime;

        if (_cooldownUpdateUI > 0f)
            return;

        UpdateUI();
        _cooldownUpdateUI = 0.5f;
    }

    private void HandleResearch()
    {
        if (_researchTime > 0)
        {
            _researchTime -= Time.deltaTime * DEBUG_researchMultiplierSpeed;
            return;
        }

        currentMode = Mode.Idle;
        foreach (var upgrade in Upgradeables[_currentIndex].upgradePerks)
        {
            if (upgrade == currentlyUpgrade) continue;
            Hypatios.Player.RemoveStatusEffectGroup(upgrade);
        }
            
        audio_researchCompleted?.Play();
        Hypatios.Dialogue.QueueDialogue($"Research completed: {currentlyUpgrade.GetDisplayText()}.", "SYSTEM", 3f, shouldOverride: true);
        currentlyUpgrade.AddStatusEffectPlayer();
        currentlyUpgrade = null;
    }

    private void UpdateUI()
    {
        RefreshData();

        if (currentMode == Mode.Idle)
        {
            var weaponUpgradeable = Upgradeables[0];
            var armorUpgradeable = Upgradeables[1];

            if (weaponUpgradeable.HasLevelMaxed() && touch_weapon.gameObject.activeSelf == true) touch_weapon.gameObject.SetActive(false); 
                else if (weaponUpgradeable.HasLevelMaxed() == false && touch_weapon.gameObject.activeSelf == false) touch_weapon.gameObject.SetActive(true);

            if (armorUpgradeable.HasLevelMaxed() && touch_armor.gameObject.activeSelf == true) touch_armor.gameObject.SetActive(false); 
                else if (armorUpgradeable.HasLevelMaxed() == false && touch_armor.gameObject.activeSelf == false) touch_armor.gameObject.SetActive(true);

            touch_weapon.interactDescription = $"{weaponUpgradeable.baseSoul + weaponUpgradeable.CurrentLevel * weaponUpgradeable.perLevel_Soul} Soul";
            touch_armor.interactDescription = $"{armorUpgradeable.baseSoul + armorUpgradeable.CurrentLevel * armorUpgradeable.perLevel_Soul} Soul";

            if (monitor_SelectUpgrade.activeSelf == false)
            {
                monitor_SelectUpgrade.gameObject.SetActive(true);
                monitor_Research.gameObject.SetActive(false);
            }
        }
        else
        {
            float percent = 100 - Mathf.Floor(_researchTime / _originalResearchTime * 100);
            label_Progress.text = $"RESEARCHING...{percent}%";
            label_Upgradename.text = $"[{currentlyUpgrade.GetDisplayText()}]";

            if (monitor_Research.activeSelf == false)
            {
                monitor_SelectUpgrade.gameObject.SetActive(false);
                monitor_Research.gameObject.SetActive(true);
            }
        }
    }

    private void RefreshData()
    {
        foreach(var upgradeable in Upgradeables)
        {
            upgradeable.Refresh();
        }
    }


    public void UpgradeSomething(int index)
    {
        RefreshData();
        var upgradeableUnit = Upgradeables[index];
        var researchTarget = upgradeableUnit.GetResearchTarget();

        if (upgradeableUnit.HasLevelMaxed())
        {
            Hypatios.Dialogue.QueueDialogue($"Upgrade maxed out.", "SYSTEM", 3f, shouldOverride: true);
            return;
        }

        if (allResearchFacilities.Find(x => x.currentlyUpgrade == researchTarget) != null)
        {
            Hypatios.Dialogue.QueueDialogue($"{researchTarget.GetDisplayText()} has already under going research. Select another one.", "SYSTEM", 4f, shouldOverride: true);
            return;
        }

        int price = (int)(upgradeableUnit.baseSoul + upgradeableUnit.CurrentLevel * upgradeableUnit.perLevel_Soul);

        if (Hypatios.Game.SoulPoint < price)
        {
            Hypatios.Dialogue.QueueDialogue($"Not enough souls! Required: {price} souls.", "SYSTEM", 3f, shouldOverride: true);
            return;
        }

        Hypatios.Game.SoulPoint -= price;
        float _timeToResearch = upgradeableUnit.baseTime + upgradeableUnit.CurrentLevel * upgradeableUnit.perLevel_Time;

        currentlyUpgrade = researchTarget;
        DeadDialogue.PromptNotifyMessage_Mod($"Research commencing: {researchTarget.GetDisplayText()}.", 6f);
        currentMode = Mode.Researching;
        _researchTime = _timeToResearch;
        _originalResearchTime = _timeToResearch;
        _currentIndex = index;
        UpdateUI();

    }

}
