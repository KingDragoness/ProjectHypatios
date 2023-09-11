using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class InstantiateRandomObject : MonoBehaviour
{

    [System.Serializable]
    public class Entry
    {
        public GameObject prefab;
        public int weight = 1;
    }

    [ShowIf("useRandomSpawn")] public RandomSpawnArea randomPositioner;
    [HideIf("useRandomSpawn")] public List<Transform> SpawnList = new List<Transform>();
    public List<GameObject> prefabs;
    public List<Entry> prefabsWithChance;
    public bool useRandomSpawn = false;

    #region Spawn With Chance
    public int GetTotalWeight()
    {
        int total = 0;
        foreach (var entry1 in prefabsWithChance)
        {
            total += entry1.weight;
        }
        return total;
    }
    public Entry GetEntry()
    {
        int output = 0;

        //Getting a random weight value
        var totalWeight = GetTotalWeight();
        int rndWeightValue = Random.Range(1, totalWeight + 1);

        //Checking where random weight value falls
        var processedWeight = 0;
        int index1 = 0;
        foreach (var entry in prefabsWithChance)
        {
            processedWeight += entry.weight;
            if (rndWeightValue <= processedWeight)
            {
                output = index1;
                break;
            }
            index1++;
        }

        return prefabsWithChance[output];
    }

    public GameObject SpawnWithChanceThing()
    {
        var go = GetEntry();

        if (useRandomSpawn)
        {
            var go1 = Instantiate(go.prefab, randomPositioner.GetAnyPositionInsideBox(), go.prefab.transform.rotation);
            IntializedSpawn(go1);
            return go1;
        }
        else
        {
            var t = SpawnList[Random.Range(0, SpawnList.Count)];
            var go1 = Instantiate(go.prefab, t.transform.position, t.rotation);
            IntializedSpawn(go1);
            return go1;
        }
    }

    #endregion

    public GameObject SpawnThing()
    {
        var go = prefabs[Random.Range(0, prefabs.Count)];

        if (useRandomSpawn)
        {
            var go1 = Instantiate(go, randomPositioner.GetAnyPositionInsideBox(), go.transform.rotation);
            IntializedSpawn(go1);
            return go1;
        }
        else
        {
            var t = SpawnList[Random.Range(0, SpawnList.Count)];
            var go1 = Instantiate(go, t.transform.position, t.rotation);
            IntializedSpawn(go1);
            return go1;
        }
    }

    public void SpawnAllFromPrefabsList()
    {
        for (int x = 0; x < prefabs.Count; x++)
        {
            var go = prefabs[x];
            if (go == null) continue;

            if (useRandomSpawn)
            {
                var go1 = Instantiate(go, randomPositioner.GetAnyPositionInsideBox(), go.transform.rotation);
                IntializedSpawn(go1);
            }
            else
            {
                var t = SpawnList[Random.Range(0, SpawnList.Count)];
                var go1 = Instantiate(go, t.transform.position, t.rotation);
                IntializedSpawn(go1);
            }
        }
    }

    //Because stupid UnityEvent cannot call function with return
    public void SpawnThing1()
    {
        SpawnThing();
    }


    public void SpawnThing(GameObject prefab1)
    {
        if (useRandomSpawn)
        {
            var go1 = Instantiate(prefab1, randomPositioner.GetAnyPositionInsideBox(), prefab1.transform.rotation);
            IntializedSpawn(go1);

        }
        else
        {
            var t = SpawnList[Random.Range(0, SpawnList.Count)];
            var go1 = Instantiate(prefab1, t.transform.position, t.rotation);
            IntializedSpawn(go1);
        }
    }

    private void IntializedSpawn(GameObject go1)
    {
        go1.SetActive(true);
    }

}
