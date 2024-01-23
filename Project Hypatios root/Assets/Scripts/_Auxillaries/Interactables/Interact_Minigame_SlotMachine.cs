using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class Interact_Minigame_SlotMachine : MonoBehaviour
{

    public enum Stage
    {
        Idle,
        Spinning,
        TransitionToStart
    }

    [System.Serializable]
    public class Level
    {
        public int lv = 0;
        public int soulCost = 5;
        public LootTable lootTable;
    }

    [FoldoutGroup("UI")] public Transform parent;
    [FoldoutGroup("UI")] public Text label_Congrats;
    [FoldoutGroup("UI")] public Text label_BetterLuckNextTime;
    [FoldoutGroup("UI")] public Text label_spinning;
    [FoldoutGroup("UI")] public List<SlotMachine_PanelSlot> slotMachine = new List<SlotMachine_PanelSlot>();
    public BaseStatValue stat_SoulSpentGambling;
    public List<Level> allLevels = new List<Level>();
    public LootTable currentLootTable;
    public AudioSource audio_Loop;
    public AudioSource audio_Success;
    public AudioSource audio_Fail;
    public GameObject titleScreen;
    public GameObject chooseModeScreen;
    public GameObject stopButton;
    public Animator anim_monitorScreen;
    public Animator anim_playScreen;
    public Stage currentStage;
    public float TimePerSlot = 2.5f;
    public float TimeToReturnStart = 3f;
    public float RandomizedWeaponCooldown = 0.02f;
    public int limitUniqueWeapons = 6;

    private bool slot1_ready = false;
    private bool slot2_ready = false;
    private bool slot3_ready = false;
    [ShowInInspector] [ReadOnly] private List<ItemInventory> currentWeaponsInSlot = new List<ItemInventory>();

    private float _timerRandomizer = 0f;
    private IEnumerator coroutine;

    private void Start()
    {
        titleScreen.gameObject.SetActive(true);
        chooseModeScreen.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (Time.timeScale <= 0) return;

        if (currentStage == Stage.Idle)
        {
            if (label_spinning.gameObject.activeSelf) label_spinning.gameObject.SetActive(false);
            if (label_BetterLuckNextTime.gameObject.activeSelf) label_BetterLuckNextTime.gameObject.SetActive(false);
            if (label_Congrats.gameObject.activeSelf) label_Congrats.gameObject.SetActive(false);
            if (stopButton.gameObject.activeSelf) stopButton.gameObject.SetActive(false);
            if (audio_Loop?.isPlaying == true) audio_Loop.Stop();
            slot1_ready = false;
            slot2_ready = false;
            slot3_ready = false;
            return;
        }

        if (currentStage == Stage.Spinning)
        {
            if (label_spinning.gameObject.activeSelf == false) label_spinning.gameObject.SetActive(true);
            if (stopButton.gameObject.activeSelf == false) stopButton.gameObject.SetActive(true);
            if (audio_Loop?.isPlaying == false) audio_Loop.Play();

        }
        else if (currentStage == Stage.TransitionToStart)
        {
            if (label_spinning.gameObject.activeSelf) label_spinning.gameObject.SetActive(false);
            if (stopButton.gameObject.activeSelf) stopButton.gameObject.SetActive(false);
            if (audio_Loop?.isPlaying == true) audio_Loop.Stop();

        }

        _timerRandomizer -= Time.deltaTime;

        if (_timerRandomizer <= 0)
        {
            if (slot1_ready == false) RandomizeSlotIcon(0);
            if (slot2_ready == false) RandomizeSlotIcon(1);
            if (slot3_ready == false) RandomizeSlotIcon(2);

            _timerRandomizer = RandomizedWeaponCooldown;
        }
    }

    private void OnEnable()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            StartCoroutine(coroutine);
        }

        if (currentStage == Stage.Idle | currentStage == Stage.TransitionToStart)
        {
            StopAllCoroutines();
            titleScreen.gameObject.SetActive(true);
            chooseModeScreen.gameObject.SetActive(true);
            currentStage = Stage.Idle;
        }
        else if (currentStage == Stage.Spinning)
        {
            anim_monitorScreen.SetTrigger("Start");
            anim_playScreen.SetTrigger("Start");
            currentStage = Stage.Spinning;
            _timerRandomizer = RandomizedWeaponCooldown;
        }
    }

    private void RandomizeSlotIcon(int i)
    {
        var item = currentWeaponsInSlot[Random.Range(0, currentWeaponsInSlot.Count - 1)];
        slotMachine[i].item = item;
        slotMachine[i].weaponIcon.sprite = item.attachedWeapon.weaponIcon;
    }

    public void TriggerSlot(int level = 0)
    {
        var _Targetlevel = allLevels[level];

        if (Hypatios.Game.SoulPoint < _Targetlevel.soulCost)
        {
            Hypatios.Dialogue.QueueDialogue($"You don't have enough souls to play this game.", "SYSTEM", 5f, dontQueue: true);
            return;
        }

        if (currentStage == Stage.Spinning | currentStage == Stage.TransitionToStart)
        {
            return;
        }

        Hypatios.Game.SoulPoint -= _Targetlevel.soulCost;
        Hypatios.Game.Add_PlayerStat(stat_SoulSpentGambling, _Targetlevel.soulCost);

        anim_monitorScreen.SetTrigger("Start");
        anim_playScreen.SetTrigger("Start");
        currentLootTable = _Targetlevel.lootTable;
        currentStage = Stage.Spinning;
        _timerRandomizer = RandomizedWeaponCooldown;
   
        coroutine = SlotCounting();
        StartCoroutine(coroutine);
        InitializeWeaponSlot();
    }

    private void InitializeWeaponSlot()
    {
        currentWeaponsInSlot.Clear();

        for(int x = 0; x < limitUniqueWeapons; x++)
        {
            int totalTries = 0;
            var entry = currentLootTable.GetEntry(Random.Range(-99999, 99999));

            while (IfItemAlreadyExists(entry.item))
            {
                if (totalTries > 19) break;
                entry = currentLootTable.GetEntry(Random.Range(-99999, 99999));
                totalTries++;
            }

            currentWeaponsInSlot.Add(entry.item);
        }
    }

    private bool IfItemAlreadyExists(ItemInventory item)
    {
        return currentWeaponsInSlot.Find(x => x == item) != null ? true : false;
    }

    IEnumerator SlotCounting()
    {
        yield return new WaitForSeconds(TimePerSlot);
        titleScreen.gameObject.SetActive(false);
        chooseModeScreen.gameObject.SetActive(false);
        SetSlotReady(0);
        yield return new WaitForSeconds(TimePerSlot);
        SetSlotReady(1);
        yield return new WaitForSeconds(TimePerSlot);
        SetSlotReady(2);
        PlayerWinningState();
        currentStage = Stage.TransitionToStart;
        yield return new WaitForSeconds(TimeToReturnStart);
        currentStage = Stage.Idle;
        titleScreen.gameObject.SetActive(true);
        chooseModeScreen.gameObject.SetActive(true);
    }

    IEnumerator ImmediateExit()
    {
        chooseModeScreen.gameObject.SetActive(false);
        titleScreen.gameObject.SetActive(false);
        yield return new WaitForSeconds(TimeToReturnStart);
        currentStage = Stage.Idle;
        titleScreen.gameObject.SetActive(true);
        chooseModeScreen.gameObject.SetActive(true);
    }

    public void ForceStop()
    {
        SetSlotReady(0);
        SetSlotReady(1);
        SetSlotReady(2);
        PlayerWinningState();
        StopCoroutine(coroutine);
        coroutine = ImmediateExit();
        StartCoroutine(coroutine);
        currentStage = Stage.TransitionToStart;
    }

    private void PlayerWinningState()
    {
        bool isPlayerWin = true;
        ItemInventory _currentItem = null;

        for(int x = 0; x < slotMachine.Count; x++)
        {
            if (_currentItem == null)
            {
                _currentItem = slotMachine[x].item;
            }

            if (_currentItem != slotMachine[x].item)
            {
                isPlayerWin = false;
                break;
            }
        }

        if (isPlayerWin)
        {
            var itemClass = Hypatios.Assets.GetItem(_currentItem.GetID());
            var itemDat = Hypatios.Player.Inventory.AddItem(_currentItem, 1);
            itemDat.RewardAmmo(5);

            DeadDialogue.PromptNotifyMessage_Mod($"You won a {_currentItem.GetDisplayText()}.", 4f);
            audio_Success?.Play();
        }
        else
        {
            audio_Fail?.Play();
        }

        if (isPlayerWin == false) label_BetterLuckNextTime.gameObject.SetActive(true);
        else label_Congrats.gameObject.SetActive(true);
    }


    public void SetSlotReady(int i)
    {
        if (i == 0)
            slot1_ready = true;
        else if (i == 1)
            slot2_ready = true;
        else if (i == 2)
            slot3_ready = true;
    }


}
