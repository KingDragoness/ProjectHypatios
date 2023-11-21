using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
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

    public int startingEnemyCount = 10;
    public int startingEnemyMax = 4;
    public List<Gauntlet_ChamberRoom> allChamberRooms = new List<Gauntlet_ChamberRoom>();
    public List<EnemySpawnStat> enemyMasterlist = new List<EnemySpawnStat>();
    public GameEvent OnResetChamber;
    public UnityEvent OnGauntletStarted;
    public UnityEvent OnGauntletCompleted;
    public Gauntlet_ChamberRoom currentChamber;
    [FoldoutGroup("UI")] public Text label_Waves;
    [FoldoutGroup("UI")] public GameObject waveUI;
    public int wave = 1;

    public static GauntletScript Instance;
    private List<Gauntlet_ChamberRoom> previousChambers = new List<Gauntlet_ChamberRoom>();
    private bool hasGauntletStarted = false;

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

    private void Update()
    {
        if (hasGauntletStarted == false) return;

        if (waveUI.activeSelf == false)
        {
            waveUI.gameObject.SetActive(true);
        }

        label_Waves.text = $"{wave}";
    }

    [FoldoutGroup("DEBUG")]
    [Button("Debug Instant Complete")]
    public void DEBUG_InstantComplete()
    {
        currentChamber.DEBUG_InstantComplete();
    }

    public void ChamberSuccessful()
    {
        wave++;
        OnGauntletCompleted?.Invoke();
    }

    #region Chamber Selection

    internal int GetTotalWeight()
    {
        int result = 0;

        foreach(var dungeonRoom in allChamberRooms)
        {
            result += dungeonRoom.weight;
        }

        return result;
    }

    internal Gauntlet_ChamberRoom GetEntry(int customSeed = 0)
    {
        int output = 0;
        {
            //Getting a random weight value
            var totalWeight = GetTotalWeight();
            int rndWeightValue = Random.Range(0, totalWeight);

            //Checking where random weight value falls
            var processedWeight = 0;
            int index1 = 0;
            foreach (var entry in allChamberRooms)
            {
                processedWeight += entry.weight;
                if (rndWeightValue <= processedWeight)
                {
                    output = index1;
                    break;
                }
                index1++;
            }
        }

        return allChamberRooms[output];
    }

    public void ProceedNextChamber()
    {
        foreach (var dungeon in allChamberRooms)
        {
            dungeon.gameObject.SetActive(false);
        }

        Gauntlet_ChamberRoom chamberRoom = GetEntry(Mathf.RoundToInt(Time.time));
        int attempt = 0;
        bool success = false;

        {
            while (wave == 1 && attempt < 100)
            {
                if (chamberRoom.dungeonType == Gauntlet_ChamberRoom.Type.Safehouse)
                {
                    chamberRoom = GetEntry(Mathf.RoundToInt(Time.time + attempt));
                }
                else
                {
                    break;
                }

                attempt++;
            }
            //safehouse
            {
                float random = Random.Range(0f, 1f);

                if (random < 0.8f && Is_nextRoom_Safehouse())
                {
                    var safehouses = new List<Gauntlet_ChamberRoom>();
                    safehouses.AddRange(allChamberRooms.FindAll(x => x.dungeonType == Gauntlet_ChamberRoom.Type.Safehouse));

                    chamberRoom = safehouses[Random.Range(0, safehouses.Count)];
                }
            }

            attempt = 0;

            while (attempt < 100 && success == false)
            {
                if (previousChambers.Count == 0) break;
                if (previousChambers.Count >= 1)
                {
                    if (previousChambers[0] == chamberRoom)
                    {
                        chamberRoom = GetEntry(Mathf.RoundToInt(Time.time + attempt));
                    }
                    else
                    {
                        success = true;
                    }
                }

                attempt++;
            }
        }

        int enemyAmount = startingEnemyCount + Mathf.RoundToInt(wave * 1.4f);
        int enemyMax = startingEnemyMax + Mathf.RoundToInt(wave * 0.8f);

        List<EnemyScript> enemiesToSpawn = new List<EnemyScript>();

        foreach(var enemyStat in enemyMasterlist)
        {
            if (wave < enemyStat.minimumLevel) continue;
            enemiesToSpawn.Add(enemyStat.enemy);
        }

        if (hasGauntletStarted == false)
        {
            OnGauntletStarted?.Invoke();
        }

        OnResetChamber?.Raise();
        previousChambers.Insert(0, chamberRoom);
        hasGauntletStarted = true;
        chamberRoom.StartChamber(enemiesToSpawn, _maxEnemyCount: enemyMax, _enemyAmount: enemyAmount);
    }

    [FoldoutGroup("DEBUG")] [Button("Safehouse test")]
    public void TEST_injectSafehouseTest()
    {
        float random = Random.Range(0f, 1f);

        if (random < 1f && Is_nextRoom_Safehouse())
        {
            var safehouses = new List<Gauntlet_ChamberRoom>();
            safehouses.AddRange(allChamberRooms.FindAll(x => x.dungeonType == Gauntlet_ChamberRoom.Type.Safehouse));

            var chamberRoom = safehouses[Random.Range(0, safehouses.Count)];
            Debug.Log(chamberRoom.gameObject.name);
        }
    }

    //CHECKS

    /// <summary>
    /// 3 dungeon rooms in the row
    /// </summary>
    /// <returns></returns>
    public bool Is_nextRoom_Safehouse()
    {
        if (previousChambers.Count <= 3)
            return false;

        int normalRoomCount = 0;

        for (int x = 0; x < 3; x++)
        {
            var dungeon = previousChambers[x];
            if (dungeon.dungeonType == Gauntlet_ChamberRoom.Type.Dungeon)
            {
                normalRoomCount++;
            }
            if (dungeon.dungeonType == Gauntlet_ChamberRoom.Type.Safehouse)
            {
                //there's already a safehouse, ignore
                return false;
            }
        }

        if (normalRoomCount >= 2) return true;

        return false;
    }

    #endregion


}
