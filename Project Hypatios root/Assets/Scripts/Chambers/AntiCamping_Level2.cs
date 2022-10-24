using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiCamping_Level2 : MonoBehaviour
{
    public Transform spawnArea;
    public Enemy bomb;
    public StageChamberScript chamberScript;
    public TriggerRegion triggerRegion;
    public float limitTime = 23f;

    private float timerPlayerInArea = 0f;

    private void Update()
    {
        if (triggerRegion.CheckPlayerIsInsideRegion())
        {
            timerPlayerInArea += Time.deltaTime;

            if (timerPlayerInArea > limitTime)
            {
                SpawnBomb();
                timerPlayerInArea = 0;
            }
        }
        else
        {
            timerPlayerInArea = 0f;
        }
    }


    public void SpawnBomb()
    {

        Vector3 rangeSpawn = spawnArea.position;


        var NewEnemy = Instantiate(bomb, transform);
        NewEnemy.transform.position = rangeSpawn;
        NewEnemy.gameObject.SetActive(true);
        chamberScript.AddEnemy(NewEnemy);
    }
}
