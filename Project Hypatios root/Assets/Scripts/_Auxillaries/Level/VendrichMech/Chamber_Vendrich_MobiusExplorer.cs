using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class Chamber_Vendrich_MobiusExplorer : MonoBehaviour
{

    public EnemyScript explorerPrefab;
    public MechHeavenblazerEnemy mechHeavenblazer;
    public float minHP = 30000f;
    public float maxHP = 60000f;
    public RandomSpawnArea spawnArea;
    public float SpawnTimer = 15f;
    public float chanceSpawn = 0.2f;

    private float _timer = 15f;
    private EnemyScript _currentEnemy;

    private void Update()
    {
        if (Time.timeScale <= 0)
        {
            return;
        }

        _timer -= Time.deltaTime;
        bool is_HPConditionMet = false;

        if (mechHeavenblazer.Stats.CurrentHitpoint > minHP && mechHeavenblazer.Stats.CurrentHitpoint < maxHP)
        {
            is_HPConditionMet = true;
        }

        if (_currentEnemy == null && is_HPConditionMet == true && _timer <= 0f)
        {
            float random = Random.Range(0f, 1f);

            if (random < chanceSpawn)
            {
                SpawnMobiusExplorer();
            }

        }

        if (_timer <= 0f)
        {
            _timer = SpawnTimer;
        }
    }

    private void SpawnMobiusExplorer()
    {
        Vector3 spawn = spawnArea.GetAnyPositionInsideBox();
        _currentEnemy = Instantiate(explorerPrefab, spawn, explorerPrefab.transform.rotation);
        _currentEnemy.gameObject.SetActive(true);
    }

}
