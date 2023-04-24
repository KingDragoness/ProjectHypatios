using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ChamberSewers_MiningRockGen : MonoBehaviour
{
   
    [System.Serializable]
    public class Entry
    {
        public GameObject prefab;
        public RandomSpawnArea randomSpawner;
        [ProgressBar(2, "maxRoll")]
        public int minRoll = 2;
        [ProgressBar("minRoll", 100)]
        public int maxRoll = 80;

        public float minScaleMultiplier = 0.9f;
        public float maxScaleMultiplier = 1.1f;
    }

    public Transform parent;
    public List<Entry> entries = new List<Entry>();

    private void Start()
    {
        GenerateRocks();
    }

    private void GenerateRocks()
    {

        foreach (var entry in entries)
        {
            int roll = Random.Range(entry.minRoll, entry.maxRoll);

            for (int x = 0; x < roll; x++)
            {
                var prefab1 = Instantiate(entry.prefab);
                Vector3 pos = entry.randomSpawner.GetAnyPositionInsideBox();
                Vector3 rot = Vector3.zero;
                rot.x = Random.Range(0f, 360f);
                rot.y = Random.Range(0f, 360f);
                rot.z = Random.Range(0f, 360f);
                prefab1.transform.position = pos;
                prefab1.transform.eulerAngles = rot;
                prefab1.transform.localScale *= Random.Range(entry.minScaleMultiplier, entry.maxScaleMultiplier);
                prefab1.SetActive(true);
                prefab1.transform.SetParent(parent);
            }
        }
    }

    public int GetTotalWeight()
    {
        int total = 0;
        foreach (var entry1 in entries)
        {
            total += 0;
        }
        return total;
    }

    public Entry GetEntry()
    {
        int output = 0;

        //Getting a random weight value
        var totalWeight = GetTotalWeight();

        var rndWeightValue = Random.Range(1, totalWeight + 1);

        //Checking where random weight value falls
        var processedWeight = 0;
        int index1 = 0;
        foreach (var entry in entries)
        {
            processedWeight += entry.minRoll;
            if (rndWeightValue <= processedWeight)
            {
                output = index1;
                break;
            }
            index1++;
        }

        return entries[output];
    }
}
