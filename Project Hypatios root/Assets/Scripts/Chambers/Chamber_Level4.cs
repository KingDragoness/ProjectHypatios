using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chamber_Level4 : MonoBehaviour
{

    public List<InstantiateRandomObject> enemySpawners;
    public int leftEnemy = 15;
    public Transform spawnTransform;
    public float range = 10;

    [Header("References")]
    public StageChamberScript chamberScript;

    private float cooldownCheck = 1f;

    public void Update()
    {
        cooldownCheck -= Time.deltaTime;

        if (cooldownCheck < 0)
        {
            if (chamberScript.enemiesToClear.Count <= 4 && leftEnemy > 0)
            {
                SpawnEnemy();
            }

            //special



            if (leftEnemy == 12)
            {
                SpawnEnemy();
                SpawnEnemy();
                SpawnEnemy();
                SpawnEnemy();


            }

            if (leftEnemy == 9)
            {
                SpawnEnemy();
                SpawnEnemy();
                SpawnEnemy();
                SpawnEnemy();

            }

            if (leftEnemy == 3)
            {
                SpawnEnemy();
                SpawnEnemy();
            }

            cooldownCheck = 1;
        }
    }

    [ContextMenu("SpawnEnemy")]
    public void SpawnEnemy()
    {
        leftEnemy--;

        Vector3 rangeSpawn = spawnTransform.position;
        rangeSpawn.x += Random.Range(-range / 2, range / 2);
        rangeSpawn.z += Random.Range(-range / 2, range / 2);

        InstantiateRandomObject spawner = enemySpawners[Random.Range(0, enemySpawners.Count - 1)];

        var NewEnemy = spawner.SpawnWithChanceThing().GetComponent<EnemyScript>();
        NewEnemy.gameObject.SetActive(true);
        chamberScript.AddEnemy(NewEnemy);
    }
}
