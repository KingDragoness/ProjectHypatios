using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chamber_Level1 : MonoBehaviour
{

    public List<Enemy> enemyToSpawn;
    public int leftEnemy = 8;
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
            if (chamberScript.enemiesToClear.Count <= 2 && leftEnemy > 0)
            {
                SpawnEnemy();
            }

            //special

            if (leftEnemy == 7)
            {
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
        rangeSpawn.x += Random.Range(-range/2, range/2);
        rangeSpawn.z += Random.Range(-range/2, range/2);

        var NewEnemy = Instantiate(enemyToSpawn[0], transform);
        NewEnemy.transform.position = rangeSpawn;
        NewEnemy.gameObject.SetActive(true);
        chamberScript.AddEnemy(NewEnemy);
    }

}
