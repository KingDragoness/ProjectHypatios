using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public abstract class Interact_Generic_Casino : MonoBehaviour
{
    [System.Serializable]
    public class Chip
    {
        public Interact_Casino_WagerToken chipObject;
        public Transform spawnArea;
        public TextMesh label_CountChipTaken;
        public int soul = 10;
        public int spawnTotal = 10;
        [ShowInInspector] [ReadOnly] internal List<Interact_Casino_WagerToken> allSpawnedChips = new List<Interact_Casino_WagerToken>();
    }

    public float distanceChip = 0.05f;
    public float distanceChip_side = 0.1f;
    public List<Chip> TokenSpawns = new List<Chip>();
    public TextMesh label_TotalWagerSouls;
    public BaseStatValue stat_SoulSpentGambling;
    public UnityEvent OnRefresh;
    public UnityEvent OnStartMinigame;
    public UnityEvent OnRestartMinigame;

    internal bool _isBetLocked = false;
    [ShowInInspector] [ReadOnly] internal int _totalSoul = 0;

    [ShowInInspector] [ReadOnly] private List<Interact_Casino_WagerToken> allTakenChips = new List<Interact_Casino_WagerToken>();


    public virtual void Start()
    {
        GameObject _tokenContainer = new GameObject("Wager Chips");
        _tokenContainer.transform.SetParent(transform);

        int ID = 0;

        foreach (var chip in TokenSpawns)
        {
            for (int x = 0; x < chip.spawnTotal; x++)
            {
                var prefab1 = Instantiate(chip.chipObject, _tokenContainer.transform);
                Vector3 pos = chip.spawnArea.position;

                {
                    int a = Mathf.FloorToInt(x / 8);
                    pos.y += distanceChip * (x - a * 8);

                    if (a >= 1)
                    {
                        pos.x += distanceChip_side * a;
                    }
                }

                prefab1.transform.position = pos;
                prefab1.casinoWagerScript = this;
                prefab1.gameObject.SetActive(true);
                prefab1.ID = ID;
                prefab1.OverrideWagerToken();

                chip.allSpawnedChips.Add(prefab1);
            }

            ID++;
        }
    }


    #region Functions
    public Chip GetChipStat(int ID)
    {
        return TokenSpawns[ID];
    }

    public int TotalSoul()
    {
        int soul = 0;

        foreach(var chip in allTakenChips)
        {
            Chip chipStat = GetChipStat(chip.ID);
            soul += chipStat.soul;
        }

        return soul;
    }

    public void TakeChip(Interact_Casino_WagerToken chip)
    {
        if (_isBetLocked == false) allTakenChips.Add(chip); 
        else
        {
            Prompt_BetLockOn();
        }
        ChipRefresh();
    }

    public void RemoveChip(int ID)
    {
        var chip = allTakenChips.Find(x => x.ID == ID);
        if (chip == null) return;

        if (_isBetLocked == false) allTakenChips.Remove(chip);
        else
        {
            Prompt_BetLockOn();
        }
        ChipRefresh();
    }

    public int GetChipCount(int ID)
    {
        int count = 0;


        foreach(var _thisChip in allTakenChips)
        {
            if (_thisChip.ID == ID)
                count++;
        }

        return count;
    }

    public virtual void ChipRefresh()
    {
        foreach (var chip in TokenSpawns)
        {
            foreach (var spawnedChip in chip.allSpawnedChips)
            {
                spawnedChip.gameObject.SetActive(true);
            }
        }

        foreach (var chip1 in allTakenChips)
        {
            chip1.gameObject.SetActive(false);
        }

        //Refresh chip positions
        {
            foreach (var chip in TokenSpawns)
            {
                var listWithoutTaken = new List<Interact_Casino_WagerToken>();
                listWithoutTaken.AddRange(chip.allSpawnedChips);
                listWithoutTaken.RemoveAll(x => allTakenChips.Contains(x) == true);

                for (int x = 0; x < listWithoutTaken.Count; x++)
                {
                    var prefab1 = listWithoutTaken[x];
                    Vector3 pos = chip.spawnArea.position;

                    {
                        int a = Mathf.FloorToInt(x / 8);
                        pos.y += distanceChip * (x - a * 8);

                        if (a >= 1)
                        {
                            pos.x += distanceChip_side * a;
                        }
                    }

                    prefab1.transform.position = pos;
                }
            }
        }

        _totalSoul = TotalSoul();
        OnRefresh?.Invoke();
    }

    #endregion

    //focus on TV's monitor UI
    #region Wager Monitor TV
    private float _cooldown = 0.2f;

    private void Update()
    {
        _cooldown -= Time.deltaTime;

        if (_cooldown > 0f) return;
        RefreshUI();
        _cooldown = 0.2f;
    }

    private void RefreshUI()
    {
        int ID = 0;

        foreach (var chip in TokenSpawns)
        {
            int count = GetChipCount(ID);
            chip.label_CountChipTaken.text = $"x{count}";
            ID++;
        }

        label_TotalWagerSouls.text = $"{TotalSoul()} Souls";
    }

    #endregion

    [FoldoutGroup("DEBUG")] [Button("Clear Chips")]
    public void DEBUG_ClearChips()
    {
        allTakenChips.Clear();
        ChipRefresh();
    }

    private void Prompt_BetLockOn()
    {
        DeadDialogue.PromptNotifyMessage_Mod("Bet is locked. You cannot add wager anymore.", 4f);
    }

    #region Base 

    public void StartGambling()
    {
        LockOnBet();
    }

    public virtual bool LockOnBet()
    {
        if (_isBetLocked == true)
        {
            DeadDialogue.PromptNotifyMessage_Mod("You already put the wager. You have to wait the game to finish.", 4f);
            return false;
        }

        if (_totalSoul <= 0)
        {
            DeadDialogue.PromptNotifyMessage_Mod("No wager! Take the casino chips to add wager!", 4f);
            return false;
        }

        if (Hypatios.Game.SoulPoint < _totalSoul)
        {
            DeadDialogue.PromptNotifyMessage_Mod($"Not enough souls! {_totalSoul} souls required.", 4f);
            return false;
        }

        Hypatios.Game.Add_PlayerStat(stat_SoulSpentGambling, _totalSoul);
        _isBetLocked = true;
        OnStartMinigame?.Invoke();
        Hypatios.Game.SoulPoint -= _totalSoul;

        return true;
    }

    #endregion
}
