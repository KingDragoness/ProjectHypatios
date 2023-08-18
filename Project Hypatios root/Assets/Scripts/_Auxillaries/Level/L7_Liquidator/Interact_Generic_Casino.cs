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
        public int soul = 10;
        public int spawnTotal = 10;
        [ShowInInspector] [ReadOnly] internal List<Interact_Casino_WagerToken> allSpawnedChips = new List<Interact_Casino_WagerToken>();
    }

    public float distanceChip = 0.05f;
    public float distanceChip_side = 0.1f;
    public List<Chip> TokenSpawns = new List<Chip>();
    public UnityEvent OnRefresh;
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

    public void RemoveChip(Interact_Casino_WagerToken chip)
    {
        if (_isBetLocked == false) allTakenChips.Remove(chip);
        else
        {
            Prompt_BetLockOn();
        }
        ChipRefresh();
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

        _totalSoul = TotalSoul();
        OnRefresh?.Invoke();
    }

    private void Prompt_BetLockOn()
    {
        DeadDialogue.PromptNotifyMessage_Mod("Bet is locked. You cannot add wager anymore.", 4f);
    }

    public virtual void LockOnBet()
    {
        _isBetLocked = true;
    }
}
