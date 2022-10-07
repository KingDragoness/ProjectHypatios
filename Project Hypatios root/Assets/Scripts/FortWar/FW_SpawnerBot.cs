using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FW_SpawnerBot : MonoBehaviour
{

    public FW_Alliance alliance = FW_Alliance.DEFENDER;
    public Chamber_Level7 chamberScript;
    public Transform spawnPoint;

    int numUnit = 0;

    private void Start()
    {
        int i = 0;

        if (alliance == FW_Alliance.DEFENDER) return;

        while (i < Chamber_Level7.MAXIMUM_PLAYER_TEAM - 1)
        {
            SpawnUnit();
            i++;
        }
    }

    private void Update()
    {
        if (chamberScript.currentStage != Chamber_Level7.Stage.Ongoing) return;

        if (alliance == FW_Alliance.DEFENDER)
        {
            int currentCount = chamberScript.DefenderUnitCount;

            if (currentCount < Chamber_Level7.MAXIMUM_PLAYER_TEAM)
            {
                PrepareSpawn();
            }
        }
        else if (alliance == FW_Alliance.INVADER)
        {
            int currentCount = chamberScript.InvaderUnitCount;

            if (currentCount < Chamber_Level7.MAXIMUM_PLAYER_TEAM)
            {
                PrepareSpawn();
            }
        }
    }

    private float _timerSpawn = 3f;
    private float _timeToSpawn = 3f;

    public void SetTimerSpawn(float time = 4f)
    {
        _timeToSpawn = time;
    }

    private void PrepareSpawn()
    {
 
        if (_timerSpawn > 0)
        {
            _timerSpawn -= Time.deltaTime;
        }
        else
        {
            SpawnUnit();
            _timerSpawn = _timeToSpawn;
        }
    }

    private void SpawnUnit()
    {
        var prefabTarget = chamberScript.defenderGuardstar;

        if (alliance == FW_Alliance.INVADER)
        {
            prefabTarget = chamberScript.invaderGuardstar;
        }

        var newPrefab1 = Instantiate(prefabTarget);
        Vector3 offset = new Vector3();
        offset.x += Random.Range(-5f, 5f);
        offset.z += Random.Range(-5f, 5f);

        newPrefab1.gameObject.SetActive(true);
        newPrefab1.Agent.Warp(spawnPoint.position);
        newPrefab1.gameObject.name = $"{numUnit}_{newPrefab1.myUnit.Alliance}";
        newPrefab1.transform.position = spawnPoint.position + offset;
        numUnit++;

    }
}
