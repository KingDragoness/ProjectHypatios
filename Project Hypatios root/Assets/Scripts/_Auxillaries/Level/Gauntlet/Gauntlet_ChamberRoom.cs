using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class Gauntlet_ChamberRoom : MonoBehaviour
{

    public enum Type
    {
        Dungeon,
        Safehouse
    }



    [System.Serializable]
    public class Spawner
    {
        public EnemyStats.SpawnType spawnType;
        public InstantiateRandomObject enemySpawner;
        public int weight = 10;
    }


    public Transform spawnPoint;
    public Transform enemyContainer;
    public Type dungeonType;
    public List<Spawner> allSpawners = new List<Spawner>();
    [FoldoutGroup("Enemy Stat")] public int EnemyLeft = 13;
    [FoldoutGroup("Enemy Stat")] public int MaxEnemyCount = 5;
    [FoldoutGroup("Enemy Stat")] public List<BaseChamberScript.InjectEnemy> allInjections;
    [FoldoutGroup("References")] public Animator animator_Door;
    [FoldoutGroup("References")] public ChamberText chamberText;
    [FoldoutGroup("References")] public GameObject sign_LevelStateCleared;
    [FoldoutGroup("References")] public GameObject sign_LevelStateUnclear;
    public UnityEvent OnChamberCompleted;

    [Space]
    public List<EnemyScript> enemiesToClear;
    public float CooldownCheck = 1f;

    private bool _isRunning = false;
    private bool _isCleared = false;
    internal float _cooldownCheck = 1f;

    private GauntletScript gauntletScript
    {
        get
        {
            return GauntletScript.Instance;
        }
    }


    #region DEBUG
    public List<EnemyScript> DEBUG_AllEnemiesToSpawn = new List<EnemyScript>();

    [FoldoutGroup("DEBUG")] [Button("Start Chamber")]
    public void DEBUG_StartChamber()
    {
        StartChamber(DEBUG_AllEnemiesToSpawn);
    }

    #endregion

    #region Set up chamber
    public void StartChamber(List<EnemyScript> enemies, int _maxEnemyCount = 5, int _enemyAmount = 13)
    {
        gameObject.SetActive(true);

        if (dungeonType == Type.Dungeon)
        {
            ResetChamber();
            _isRunning = true;
            _isCleared = false;

            EnemyLeft = _enemyAmount;
            MaxEnemyCount = _maxEnemyCount;
            DeadDialogue.PromptNotifyMessage_Mod($"Wave {gauntletScript.wave} started.", 5f);
            SetSpawners(enemies);

        }
        else if (dungeonType == Type.Safehouse)
        {
            _isRunning = false;
            _isCleared = true;
        }

        Hypatios.UI.mainHUDScript.FadeIn();
        Hypatios.Player.transform.position = spawnPoint.position;
        Hypatios.Player.transform.rotation = spawnPoint.rotation;
        gauntletScript.currentChamber = this;
    }
    private void ResetChamber()
    {
        enemiesToClear.Clear();
        animator_Door.SetBool("IsOpened", false);
        sign_LevelStateCleared.gameObject.SetActive(false);
        sign_LevelStateUnclear.gameObject.SetActive(true);
    }

    private void SetSpawners(List<EnemyScript> enemies)
    {
        
        foreach(var spawner in allSpawners)
        {
            spawner.enemySpawner.prefabsWithChance.Clear();
        }

        foreach(var enemyPrefab in enemies)
        {
            foreach(var spawner in allSpawners)
            {
                if (spawner.spawnType != enemyPrefab.GetRawBaseStat.Stats.spawnType) continue;

                InstantiateRandomObject.Entry entry = new InstantiateRandomObject.Entry();
                entry.prefab = enemyPrefab.gameObject;
                entry.weight = gauntletScript.GetSpawnStat(enemyPrefab).weight;

                spawner.enemySpawner.prefabsWithChance.Add(entry);
                break;
            }
        }
    }

    #endregion

    private void Update()
    {
        if (_isRunning)
        {
            RunChamber();
        }

        if (enemiesToClear.Count == 0 && EnemyLeft <= 0 && !_isCleared && _isRunning)
        {
            ClearedChamber();
        }
    }



    private void RunChamber()
    {
        _cooldownCheck -= Time.deltaTime;

        if (_cooldownCheck < 0)
        {
            HandleSpawn();
            _cooldownCheck = CooldownCheck;
            chamberText.SetTextContent(enemiesToClear.Count.ToString());
            enemiesToClear.RemoveAll(x => x == null);
        }
    }

    [FoldoutGroup("DEBUG")]
    [Button("Complete Chamber")]
    private void ClearedChamber()
    {
        animator_Door.SetBool("IsOpened", true);
        sign_LevelStateCleared.gameObject.SetActive(true);
        sign_LevelStateUnclear.gameObject.SetActive(false);
        chamberText.SetTextContent(enemiesToClear.Count.ToString());

        if (!_isCleared)
        {
            OnChamberCompleted?.Invoke();
            DeadDialogue.PromptNotifyMessage_Mod($"Wave {gauntletScript.wave} completed. Proceed to the door.", 5f);
            Hypatios.Dialogue.QueueDialogue($"Attention to all gauntlet runners: Wave {gauntletScript.wave} has been completed. Go to the door to proceed the next wave.", "ANNOUNCER", 7f);

        }

        _isCleared = true;
        _isRunning = false;
        gauntletScript.ChamberSuccessful();
    }

    private void HandleSpawn()
    {

        foreach (var injectGroup in allInjections)
        {
            if (EnemyLeft == injectGroup.enemyLeft)
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

        if (enemiesToClear.Count <= MaxEnemyCount && EnemyLeft > 0)
        {
            SpawnEnemy();
        }

    }


    [FoldoutGroup("DEBUG")]
    [Button("Debug Instant Complete")]
    public void DEBUG_InstantComplete()
    {
        ConsoleCommand.Instance.CommandInput("killall");
        EnemyLeft = 0;
        ClearedChamber();
    }


    [FoldoutGroup("DEBUG")] [Button("SpawnEnemy")]
    public void SpawnEnemy(bool ignoreCount = false)
    {
        if (ignoreCount == false) EnemyLeft--;

        Spawner _spawnerGroup = GetEntry(EnemyLeft);
        InstantiateRandomObject spawner = _spawnerGroup.enemySpawner;
        //Debug.Log(spawner.gameObject.name);

        var NewEnemy = spawner.SpawnWithChanceThing().GetComponent<EnemyScript>();


        if (NewEnemy == null)
        {
            _cooldownCheck = 0.1f;
            return;
        }


        NewEnemy.gameObject.SetActive(true);
        NewEnemy.transform.SetParent(enemyContainer);
        AddEnemy(NewEnemy);
    }

    #region Spawners
    public void AddEnemy(EnemyScript enemy)
    {
        enemiesToClear.Add(enemy);
    }

    internal int GetTotalWeight()
    {
        int total = 0;
        foreach (var entry1 in ValidSpawners())
        {
            total += entry1.weight;
        }
        return total;
    }

    private int attempt = 0;
    public List<Spawner> ValidSpawners()
    {
        List<Spawner> result = new List<Spawner>();

        foreach(var spawner in allSpawners)
        {
            if (spawner.enemySpawner.prefabsWithChance.Count == 0) continue;
            result.Add(spawner);
        }

        return result;
    }

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
            foreach (var entry in ValidSpawners())
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

        return ValidSpawners()[output];
    }
    #endregion


}
