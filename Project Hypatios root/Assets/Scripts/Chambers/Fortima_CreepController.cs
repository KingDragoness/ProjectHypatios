using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fortima_CreepController : MonoBehaviour
{

    public List<EnemyScript> enemyToClear = new List<EnemyScript>();
    public List<InstantiateRandomObject> enemiesToSpawn = new List<InstantiateRandomObject>();
    public float SpawnerTime = 60f;

    private bool _cleared = false;
    private float _spawnTimer = 60f;
    private float _refreshUpdateCheck = 0.2f;

    private void Start()
    {
        _spawnTimer = SpawnerTime;
    }

    private void Update()
    {
        _refreshUpdateCheck -= Time.deltaTime;

        if (_cleared)
        {
            _spawnTimer -= Time.deltaTime;

            if (_spawnTimer <= 0)
            {
                SpawnEnemy();
            }
        }

        if (_refreshUpdateCheck > 0)
            return;

        _refreshUpdateCheck = 0.2f;
        enemyToClear.RemoveAll(x => x == null);

        if (enemyToClear.Count == 0)
        {
            _cleared = true;
        }

     
    }

    public void SpawnEnemy()
    {
        _cleared = false;
        _spawnTimer = SpawnerTime;
        foreach(var spawner in enemiesToSpawn)
        {
            var enemy = spawner.SpawnThing().GetComponent<EnemyScript>();
            enemy.gameObject.SetActive(true);
            enemyToClear.Add(enemy);
        }
    }

}
