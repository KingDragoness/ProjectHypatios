using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class EclipseBlaz_Deathsweeper : EclipseBlaz_AIModule
{

    public List<EnemyScript> allSpawnedDeathsweepers = new List<EnemyScript>();
    public EnemyScript DeathsweeperPrefab;
    public GameObject spawnParticleFX;
    public RandomSpawnArea spawnArea;
    public int limitDeathsweepers = 6;
    public float spawnRate = 6f; //per rate, spawns 2
    [Range(0f,1f)] public float spawnChance = 0.3f;

    private float _spawnRateTimer = 0.3f;


    private void Start()
    {
        _spawnRateTimer = spawnRate;
    }

    //spawn deathsweepers

    public override void Run()
    {
        _spawnRateTimer -= Time.deltaTime;

        if (_spawnRateTimer <= 0f)
        {
            float random = Random.Range(0f, 1f);
            int countEnemy = allSpawnedDeathsweepers.Count;

            if (random < spawnChance && countEnemy < limitDeathsweepers)
            {
                SpawnDeathsweeper();
            }

            _spawnRateTimer = spawnRate;
        }
    }

    private void SpawnDeathsweeper()
    {
        int desiredSpawn = 2;

        for(int x = 0; x < desiredSpawn; x++)
        {
            var deathSweeper = Instantiate(DeathsweeperPrefab);
            var particleSpawn = Instantiate(spawnParticleFX);
            var spawnPosition = spawnArea.GetAnyPositionInsideBox();

            deathSweeper.gameObject.SetActive(true);
            particleSpawn.gameObject.SetActive(true);
            deathSweeper.transform.position = spawnPosition;
            spawnParticleFX.transform.position = spawnPosition;
        }
    }

}
