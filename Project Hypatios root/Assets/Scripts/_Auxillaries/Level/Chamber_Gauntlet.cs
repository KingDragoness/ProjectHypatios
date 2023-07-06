using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

/// <summary>
/// 
/// </summary>

//Range:
//R1 140m: despawn immediately
//R2 110m: despawn randomly
//R3 70m-110m: spawn randomly

[RequireComponent(typeof(StageChamberScript))]
public class Chamber_Gauntlet : MonoBehaviour
{

    [System.Serializable]
    public class MobSpawn
    {
        public EnemyScript enemy;
        public int weight = 10;
    }

    [FoldoutGroup("Mob Spawns")] public float ring1_Range = 140f;
    [FoldoutGroup("Mob Spawns")] public float ring2_Range = 110f;
    [FoldoutGroup("Mob Spawns")] public float ring3_Range = 70f;
    [FoldoutGroup("Mob Spawns")] public float rangeYSpawn = 4f;
    [FoldoutGroup("Mob Spawns")] public Transform spawnTransform; //ground spawn
    [FoldoutGroup("Mob Spawns")] public float spawnChance = 0.25f;
    [FoldoutGroup("Mob Spawns")] public List<MobSpawn> mobToSpawns = new List<MobSpawn>();
    [FoldoutGroup("UI")] public Text label_waveEnemyLeft;
    [FoldoutGroup("UI")] public Text label_currentWave;
    [FoldoutGroup("UI")] public Slider slider_waveEnemyLeft;
    [FoldoutGroup("UI")] public GameObject waveUI;
    public List<EnemyScript> allCurrentEnemies = new List<EnemyScript>();
    public bool isRunning = false;
    public int currentWave = 0;
    [Space]
    public int baseWaveEnemyAmount = 10;
    public int perWaveIncrease = 2;
    public StageChamberScript chamberScript;

    [FoldoutGroup("Debug")] public float DEBUG_SpawnerTestTime = 0.5f;
    [FoldoutGroup("Debug")] public bool DEBUG_StopSpawnerTest = false;

    private float _spawnTimer = 1f;
    private int _enemyLeftInWave = 0;


    #region Updates

    private void Update()
    {
        if (Time.timeScale <= 0) return;

        if (isRunning)
        {
            Update_Waves();
        }

        Update_UI();
    }

    private void Update_UI()
    {
        if (waveUI.activeSelf == false && isRunning) waveUI.gameObject.SetActive(true);
        else if (isRunning == false)
        {
            if (waveUI.activeSelf == true) DeadDialogue.PromptNotifyMessage_Mod("Wave over! Take a break to replenish everything.", 5f);
            waveUI.gameObject.SetActive(false);

        }

        int totalLeft = _enemyLeftInWave + allCurrentEnemies.Count;

        label_currentWave.text = $"Wave {currentWave}";
        label_waveEnemyLeft.text = $"{totalLeft}";
        slider_waveEnemyLeft.maxValue = baseWaveEnemyAmount + (perWaveIncrease * currentWave);
        slider_waveEnemyLeft.value = totalLeft;

        allCurrentEnemies.RemoveAll(x => x == null);
    }

    private void Update_Waves()
    {
        if (_enemyLeftInWave + allCurrentEnemies.Count <= 0)
            isRunning = false;

        _spawnTimer -= Time.deltaTime;

        if (_spawnTimer > 0f)
            return;

        if (DEBUG_StopSpawnerTest == false) SpawnerTest();
        _spawnTimer = DEBUG_SpawnerTestTime;

        if (_enemyLeftInWave > 0)
        {
            float chance = Random.Range(0f, 1f);

            if (chance < spawnChance)
            {
                var mob = GetEntry(Mathf.RoundToInt(Time.realtimeSinceStartup));
                Debug.Log($"Attempting spawning: {mob}");
                SpawnEnemy(mob.enemy);
            }
        }
    }

    #endregion

    #region Weight
    public int GetTotalWeight()
    {
        int total = 0;
        foreach (var entry1 in mobToSpawns)
        {
            total += entry1.weight;
        }
        return total;
    }

    public MobSpawn GetEntry(int customSeed = 0)
    {
        int output = 0;
        var seed = Hypatios.GetSeed() + customSeed;
        var RandomSys = new System.Random(seed);

        //Getting a random weight value
        var totalWeight = GetTotalWeight();
        int rndWeightValue = RandomSys.Next(1, totalWeight + 1);

        //Checking where random weight value falls
        var processedWeight = 0;
        int index1 = 0;
        foreach (var entry in mobToSpawns)
        {
            processedWeight += entry.weight;
            if (rndWeightValue <= processedWeight)
            {
                output = index1;
                break;
            }
            index1++;
        }

        return mobToSpawns[output];
    }

    #endregion

    [FoldoutGroup("Debug")]
    [Button("Initiate Wave")]
    public void InitiateWave()
    {
        currentWave++;
        isRunning = true;
        _enemyLeftInWave = baseWaveEnemyAmount + (perWaveIncrease * currentWave);
        DeadDialogue.PromptNotifyMessage_Mod("Initiated wave.", 5f);
    }

    private void SpawnerTest()
    {
        float _x1 = Random.Range(0f, 1f);
        float _y1 = Random.Range(0f, 1f);
        _x1 = Mathf.Lerp(ring3_Range, ring2_Range, _x1);
        _y1 = Mathf.Lerp(ring3_Range, ring2_Range, _y1);

        Transform t = Hypatios.Player.transform;
        Vector3 pos = t.position;
        pos.y += .2f;
        pos.x += _x1 * IsopatiosUtility.RandomSign();
        pos.z += _y1 * IsopatiosUtility.RandomSign();

        spawnTransform.position = pos;
    }

    [FoldoutGroup("Debug")]
    [Button("Spawn Enemy")]
    public void SpawnEnemy(EnemyScript enemyScript)
    {
        Vector3 result = new Vector3();

        if (IsopatiosUtility.CheckNavMeshWalkable(spawnTransform.position, 1f, out result))
        {

        }
        else
        {
            Debug.Log("Abort spawning enemy");
            return;
        }

        var go1 = Instantiate(enemyScript, spawnTransform.position, spawnTransform.rotation);
        allCurrentEnemies.Add(go1);
        _enemyLeftInWave--;
        go1.gameObject.SetActive(true);
    }

}
