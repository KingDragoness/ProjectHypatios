using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// 
/// </summary>
/// 
[RequireComponent(typeof(StageChamberScript))]
public abstract class BaseChamberScript : MonoBehaviour
{

    [System.Serializable]
    public class Spawner
    {
        public int weight = 10;
        public List<InstantiateRandomObject> enemySpawners;
    }

    [System.Serializable]
    public class UpperLevelStat
    {
        public int chamberCompleted = 3;
        public ChamberStat chamberStat;
    }

    /// <summary>
    /// When enemy is 20 left, drop 4 enemies at once
    /// </summary>
    [System.Serializable]
    public class InjectEnemy
    {
        [HorizontalGroup("Extra Spawns")] [LabelWidth(60)] public int enemyLeft = 20;
        [HorizontalGroup("Extra Spawns")] [LabelWidth(60)] public int totalSpawn = 4;
    }

    [System.Serializable]
    public class ChamberStat
    {
        public int TotalEnemy = 20;
        [LabelText("Min/Max Enemy")]
        [HorizontalGroup("1")]  public int MinEnemy = 2;
        [HorizontalGroup("1")] [HideLabel] public int MaxEnemy = 8;
        public List<Spawner> AllSpawners = new List<Spawner>();
        [TableList(NumberOfItemsPerPage =3)] public List<InjectEnemy> allInjections = new List<InjectEnemy>();

        public ChamberStat Copy()
        {
            ChamberStat cs = new ChamberStat();
            cs.TotalEnemy = TotalEnemy;
            cs.MinEnemy = MinEnemy;
            cs.MaxEnemy = MaxEnemy;
            cs.AllSpawners = AllSpawners;
            cs.allInjections = allInjections;
            return cs;
        }
    }

    [SerializeField] [HideInPlayMode] private ChamberStat _baseStat; //Dont use this, use _stat
    [SerializeField] [HideInPlayMode] private List<UpperLevelStat> _upperLevelStats;
    public StageChamberScript chamberScript;
    public bool IsRunning = false;
    public float CooldownCheck = 1f;

    [ReadOnly] [HideInEditorMode] [ShowInInspector] private ChamberStat currentStat;
    internal float _cooldownCheck = 1f;

    public ChamberStat Stats { get => currentStat;  }


    #region Tools, external events
    public void ActivateRun()
    {
        IsRunning = true;
    }
    public void SpawnEnemySpecific(int index)
    {
        Spawner _spawnerGroup = currentStat.AllSpawners[index];
        InstantiateRandomObject spawner = _spawnerGroup.enemySpawners[Random.Range(0, _spawnerGroup.enemySpawners.Count - 1)];

        var NewEnemy = spawner.SpawnWithChanceThing().GetComponent<EnemyScript>();
        NewEnemy.gameObject.SetActive(true);
        chamberScript.AddEnemy(NewEnemy);
    }
    #endregion

    #region Spawners
    internal int GetTotalWeight()
    {
        int total = 0;
        foreach (var entry1 in currentStat.AllSpawners)
        {
            total += entry1.weight;
        }
        return total;
    }

    private int attempt = 0;

    internal Spawner GetEntry(int customSeed = 0)
    {
        int output = 0;
        {
            //var seed = Hypatios.GetSeed() + customSeed + attempt;
            //var RandomSys = new System.Random(seed);

            //Getting a random weight value
            var totalWeight = GetTotalWeight();
            int rndWeightValue = Random.Range(0, totalWeight);

            //Checking where random weight value falls
            var processedWeight = 0;
            int index1 = 0;
            foreach (var entry in currentStat.AllSpawners)
            {
                processedWeight += entry.weight;
                if (rndWeightValue <= processedWeight)
                {
                    output = index1;
                    break;
                }
                attempt++;
                index1++;
            }
        }

        return currentStat.AllSpawners[output];
    }
    #endregion

    public virtual void Awake()
    {
        currentStat = _baseStat.Copy();
        chamberScript = GetComponent<StageChamberScript>();
    }

    [Button("Override Chamber completion")]
    public void OverrideCurrentStat(int completion)
    {
        Hypatios.Chamber.Debug_SetChamberCompletion(completion);
        CheckCondition();
    }

    public virtual void Start()
    {
        CheckCondition();
    }

    //potential bug in the future, not checked throughly
    private void CheckCondition()
    {
        int currentRun = Hypatios.Chamber.GetChamberCompletion(); 
        int highestEntry = 0;
        UpperLevelStat _currentStat = null;

        foreach (var stat in _upperLevelStats)
        {
            if (currentRun >= stat.chamberCompleted && highestEntry <= stat.chamberCompleted)
            {
                Hypatios.Game.RuntimeTutorialHelp("Harder Chamber", "In some chambers, the difficulty may ramp up after you complete it multiple times.", "chamber.harder_difficulty");
                highestEntry = stat.chamberCompleted;
                _currentStat = stat;
            }
        }

        if (_currentStat != null)
        {
            currentStat = _currentStat.chamberStat;
        }
    }

    public virtual void Update()
    {
        if (IsRunning == false)
            return;

        _cooldownCheck -= Time.deltaTime;

        if (_cooldownCheck < 0)
        {
            HandleSpawn();
            _cooldownCheck = CooldownCheck;
        }
    }

    private void HandleSpawn()
    {

        foreach (var injectGroup in currentStat.allInjections)
        {
            if (currentStat.TotalEnemy == injectGroup.enemyLeft)
            {
                for (int x = 0; x < injectGroup.totalSpawn; x++)
                {
                    if (x == injectGroup.totalSpawn - 1)
                        SpawnEnemy();
                    else
                        SpawnEnemy(true);
                }
                break; //can only have one extra enemy spawner at a time
            }
        }

        if (chamberScript.enemiesToClear.Count <= currentStat.MinEnemy && currentStat.TotalEnemy > 0)
        {
            SpawnEnemy();
        }

    }



    [ContextMenu("SpawnEnemy")]
    public void SpawnEnemy(bool ignoreCount = false)
    {
        if (ignoreCount == false) currentStat.TotalEnemy--;

        Spawner _spawnerGroup = GetEntry(currentStat.TotalEnemy);
        InstantiateRandomObject spawner = _spawnerGroup.enemySpawners[Random.Range(0, _spawnerGroup.enemySpawners.Count)];

        var NewEnemy = spawner.SpawnWithChanceThing().GetComponent<EnemyScript>();

        NewEnemy.gameObject.SetActive(true);
        chamberScript.AddEnemy(NewEnemy);
    }

}
