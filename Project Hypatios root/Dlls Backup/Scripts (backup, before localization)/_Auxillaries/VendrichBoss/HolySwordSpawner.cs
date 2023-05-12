using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class HolySwordSpawner : MonoBehaviour
{

    public Chamber_VendrichMech chamberScript;
    public GameObject holySwordPrefab;
    public RandomSpawnArea spawnArea;
    public int spawnMaxAmountBurst = 4;
    public float burstCooldownAdditional = 0.2f;
    public float CooldownSpawnHolySword = 3.5f;
    public float CooldownAdditionalMin = -1f;
    public float CooldownAdditionalMax = 2f;
    [ShowInInspector] [ReadOnly] private List<GameObject> holySwords = new List<GameObject>();
    [Range(0,1f)] public float SpawnChance = 0.1f;

    private float _timerSpawner = 3.5f;

    private void Update()
    {
        if (chamberScript.mechEnemy.currentStage != MechHeavenblazerEnemy.Stage.Stage3_Ascend)
            return;

        if (_timerSpawner >= 0)
        {
            _timerSpawner -= Time.deltaTime;
            return;
        }

        float chance = Random.value;
        int spawnAmount = Random.Range(1, spawnMaxAmountBurst);
        if (SpawnChance > chance) StartCoroutine(SpawnHolySwords(spawnAmount));
        _timerSpawner = CooldownSpawnHolySword + Random.Range(CooldownAdditionalMin, CooldownAdditionalMax);
    }

    IEnumerator SpawnHolySwords(int amount)
    {
        int i = 0;
        while(i < amount)
        {
            SpawnHolySword();
            i++;
            yield return new WaitForSeconds(burstCooldownAdditional);
        }
    }

    public void SpawnHolySword()
    {
        var pos = spawnArea.GetAnyPositionInsideBox();
        var sword1 = Instantiate(holySwordPrefab, pos, holySwordPrefab.transform.rotation);
        sword1.gameObject.SetActive(true);
        holySwords.Add(sword1);
        holySwords.RemoveAll(x => x == null);

    }

}
