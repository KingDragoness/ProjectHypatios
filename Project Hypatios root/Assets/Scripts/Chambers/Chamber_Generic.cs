using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class Chamber_Generic : MonoBehaviour
{
   
    [System.Serializable]
    public class Spawner
    {
        public RandomSpawnArea spawnRegion;
        public List<EnemyScript> enemyPrefabs;
        [Range(0f, 1f)] public float chanceSpawn = 0.4f;
  

        public EnemyScript SpawnEnemy()
        {
            EnemyScript templateEnemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

            var newEnemy = Instantiate(templateEnemy);
            newEnemy.transform.position = spawnRegion.GetAnyPositionInsideBox();
            newEnemy.gameObject.SetActive(true);

            return newEnemy;
        }


    }

    public List<Spawner> allSpawners = new List<Spawner>();
    public int totalEnemyLeft = 20;
    public StageChamberScript chamberScript;
    public int minEnemy = 2;
    public int maxEnemy = 9;
    public float cooldownCheck = 1f;
    public bool isRun = false;

    private float _cooldownCheck = 1f;

    private void Update()
    {
        if (isRun == false)
            return;

        _cooldownCheck -= Time.deltaTime;

 

        if (_cooldownCheck < 0)
        {
            TrySpawnRandomEnemy();
            _cooldownCheck = cooldownCheck;
        }
    }

    public void ActivateRun()
    {
        isRun = true;
    }

    public void TrySpawnRandomEnemy()
    {
        if (totalEnemyLeft <= 0)
            return;


        Spawner _spawner = allSpawners[Random.Range(0, allSpawners.Count)];

        float chance = Random.Range(0f, 1f);

        if (chamberScript.enemiesToClear.Count <= minEnemy)
            chance = 0f;

        if (chamberScript.enemiesToClear.Count >= maxEnemy)
            return;

        if (chance > _spawner.chanceSpawn)
        {
            return;
        }

        var newEnemy1 =_spawner.SpawnEnemy();
        newEnemy1.transform.SetParent(transform);
        chamberScript.AddEnemy(newEnemy1);
        totalEnemyLeft--;
    }

}
