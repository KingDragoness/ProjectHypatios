using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;


[RequireComponent(typeof(StageChamberScript))]
public class GauntletScript : MonoBehaviour
{

    [System.Serializable]
    public class EnemySpawnStat
    {
        public EnemyScript enemy;
        public int minimumLevel = 1; //wave 1, wave 4, wave 8
        public int weight = 20;
    }

    public List<Gauntlet_ChamberRoom> allChamberRooms = new List<Gauntlet_ChamberRoom>();
    public List<EnemySpawnStat> enemyMasterlist = new List<EnemySpawnStat>();
    public GameEvent OnResetChamber;
    public Gauntlet_ChamberRoom currentChamber;
    public int wave = 1;

    public static GauntletScript Instance;
    private List<Gauntlet_ChamberRoom> previousChambers = new List<Gauntlet_ChamberRoom>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        foreach(var dungeon in allChamberRooms)
        {
            dungeon.gameObject.SetActive(false);
        }
    }

    public EnemySpawnStat GetSpawnStat(EnemyScript enemyScript)
    {
        return enemyMasterlist.Find(x => x.enemy == enemyScript);
    }

    [FoldoutGroup("DEBUG")]
    [Button("Debug Instant Complete")]
    public void DEBUG_InstantComplete()
    {
        currentChamber.DEBUG_InstantComplete();
    }

    public void ProceedNextChamber()
    {
        foreach (var dungeon in allChamberRooms)
        {
            dungeon.gameObject.SetActive(false);
        }

        Gauntlet_ChamberRoom chamberRoom = allChamberRooms[Random.Range(0, allChamberRooms.Count)];
        int attempt = 0;
        bool success = false;
        
        while (attempt < 100 && success == false)
        {
            if (previousChambers.Count == 0) break;
            if (previousChambers.Count >= 1)
            {
                if (previousChambers[0] == chamberRoom)
                {
                    chamberRoom = allChamberRooms[Random.Range(0, allChamberRooms.Count)];
                }
                else
                {
                    success = true;
                }
            }

            attempt++;
        }


        int enemyAmount = 15 + Mathf.RoundToInt(wave * 1.4f);
        int enemyMax = 5 + Mathf.RoundToInt(wave * 0.8f);

        List<EnemyScript> enemiesToSpawn = new List<EnemyScript>();

        foreach(var enemyStat in enemyMasterlist)
        {
            if (wave < enemyStat.minimumLevel) continue;
            enemiesToSpawn.Add(enemyStat.enemy);
        }

        OnResetChamber?.Raise();
        previousChambers.Insert(0, chamberRoom);
        chamberRoom.StartChamber(enemiesToSpawn, _maxEnemyCount: enemyMax, _enemyAmount: enemyAmount);
    }

    public void ChamberSuccessful()
    {
        wave++;
    }

}
