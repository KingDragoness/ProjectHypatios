using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chamber_Level4 : BaseChamberScript
{

    public List<InstantiateRandomObject> enemySpawners;
    public Transform spawnTransform;

    private float cooldownCheck = 1f;

    private void Additional()
    {
        #region Legacy
        //if (leftEnemy == 18)
        //{
        //    SpawnEnemy(true);
        //    SpawnEnemy(true);
        //    SpawnEnemy(true);
        //    SpawnEnemy(true);
        //    SpawnEnemy();


        //}

        //if (leftEnemy == 9)
        //{
        //    SpawnEnemy(true);
        //    SpawnEnemy(true);
        //    SpawnEnemy(true);
        //    SpawnEnemy(true);
        //    SpawnEnemy(true);
        //    SpawnEnemy(true);
        //    SpawnEnemy();

        //}

        //if (leftEnemy == 3)
        //{
        //    SpawnEnemy(true);
        //    SpawnEnemy(true);
        //    SpawnEnemy(true);
        //    SpawnEnemy(true);
        //    SpawnEnemy(true);
        //    SpawnEnemy();

        //}
        #endregion
    }

    public override void Update()
    {
        base.Update();
    }

}
